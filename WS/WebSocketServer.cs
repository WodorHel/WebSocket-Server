﻿using WS.CommandsExecutors;
using WS.Exceptions;
using WS.Protocol.Frame;
using WS.Protocol.Handshake;
using WS.SocketServices.EventArguments;
using WS.WebSocketClients;
using WS.WebSocketEventArguments;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;

namespace WS
{
    public class WebSocketServer : IWebSocketServer, IDisposable
    {
        IServerListener _server;
        IWebSocketClientsManager _webSocketClientsManager;
        IHandshakeResponseGenerator _handshakeResponseGenerator;
        IFramesManager _framesManager;
        ICommandExecutorFactory _commandsExecutorFactory;

        /// <summary>
        /// Buffer size for each client. Default is 8192
        /// </summary>
        public uint BufferSize { get; private set; }

        /// <summary>
        /// Current state of server
        /// </summary>
        public EServerState ServerState { get { return _server.ServerState; } }

        /// <summary>
        /// Event triggered when a new client has connected to the server
        /// </summary>
        public event EventHandler<WebSocketConnectedEventArgs> WebSocketConnected;

        /// <summary>
        /// Event triggered when a new data has received from connected client
        /// </summary>
        public event EventHandler<WebSocketDataReceivedEventArgs> WebSocketDataReceived;

        /// <summary>
        /// Event triggered when data has been succesfully sent
        /// </summary>
        public event EventHandler<WebSocketDataSentEventArgs> WebSocketDataSent;

        /// <summary>
        /// Event triggered when the client has disconnected from the server
        /// </summary>
        public event EventHandler<WebSocketDisconnectedEventArgs> WebSocketDisconnected;

        public WebSocketServer() : this(8192, new ServerListener(8192))
        {

        }

        public WebSocketServer(uint bufferSize) : this(bufferSize, new ServerListener(bufferSize))
        {
            
        }

        public WebSocketServer(uint bufferSize, IServerListener serverListener)
        {
            Contract.Requires<ArgumentNullException>(serverListener != null, nameof(serverListener));

            BufferSize = bufferSize;

            _server = serverListener;
            _webSocketClientsManager = new WebSocketClientsManager();
            _handshakeResponseGenerator = new HandshakeResponseGenerator();
            _framesManager = new FramesManager();
            _commandsExecutorFactory = new CommandExecutorFactory();

            _server.ClientConnected += OnConnected;
            _server.DataReceived += OnDataReceived;
            _server.DataSent += OnDataSent;
            _server.ClientDisconnected += OnDisconnected;
        }

        /// <summary>
        /// Opens WebSocket server with specified address and port.
        /// </summary>
        public void Start(IPAddress address, ushort port)
        {
            var endpoint = new IPEndPoint(address, port);
            _server.Start(endpoint);
        }

        /// <summary>
        /// Closes WebSocket server
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }

        /// <summary>
        /// Sends data (as WebSocket frame) to connected client with the specified id.
        /// Returns false if clientID not exists.
        /// </summary>
        public bool SendData(string clientID, byte[] data)
        {
            return SendData(clientID, data, FrameType.Message);
        }

        /// <summary>
        /// Sends text (as WebSocket frame) to connected client with the specified id.
        /// Returns false if clientID not exists.
        /// </summary>
        public bool SendData(string clientID, string text)
        {
            return SendData(clientID, ASCIIEncoding.UTF8.GetBytes(text), FrameType.Message);
        }

        /// <summary>
        /// Sends data (as WebSocket frame with specified type) to connected client with the specified id.
        /// Returns false if clientID not exists.
        /// </summary>
        public bool SendData(string clientID, byte[] data, FrameType type)
        {
            var webSocketClient = _webSocketClientsManager.GetByID(clientID);
            if (webSocketClient == null)
                return false;

            var frameBytes = _framesManager.Serialize(data, type);
            _server.Send(webSocketClient.Socket, frameBytes);

            return true;
        }

        /// <summary>
        /// Sends raw data (without creating WebSocket frame) to connected client with the specified id.
        /// Returns false if clientID not exists.
        /// </summary>
        public void SendRawData(string clientID, byte[] data)
        {
            var webSocketClient = _webSocketClientsManager.GetByID(clientID);
            if (webSocketClient == null)
                return;

            _server.Send(webSocketClient.Socket, data);
        }

        /// <summary>
        /// Returns informations about client with specified id. If not exists, returns null.
        /// </summary>
        public ClientInfo GetClientInfo(string clientID)
        {
            var webSocketClient = _webSocketClientsManager.GetByID(clientID);
            if (webSocketClient == null)
                return null;

            return webSocketClient.GetInfo();
        }

        /// <summary>
        /// Disconnects client with specified id.
        /// </summary>
        public void DisconnectClient(string clientID)
        {
            var webSocketClient = _webSocketClientsManager.GetByID(clientID);
            if (webSocketClient == null)
                return;

            _server.CloseConnection(webSocketClient.Socket);
            _webSocketClientsManager.Remove(webSocketClient);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_server != null) ((IDisposable)_server).Dispose();
            }
        }

        void OnConnected(object sender, ConnectedEventArgs e)
        {
            var webSocketClient = new WebSocketClient(e.Socket, BufferSize);
            _webSocketClientsManager.Add(webSocketClient);

            WebSocketConnected?.Invoke(this, new WebSocketConnectedEventArgs(webSocketClient.ID));
        }

        void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            var webSocketClient = _webSocketClientsManager.GetBySocket(e.Socket);
            if (webSocketClient == null)
                return;

            if (!webSocketClient.Buffer.Add(e.Data))
            {
                _server.CloseConnection(webSocketClient.Socket, new BufferOverflowException());
                return;
            }

            if (!webSocketClient.HandshakeDone)
                DoHandshake(webSocketClient);
            else
                ProcessMessage(webSocketClient);
        }

        void OnDataSent(object sender, DataSentEventArgs e)
        {
            var webSocketClient = _webSocketClientsManager.GetBySocket(e.Socket);
            if (webSocketClient == null)
                return;

            WebSocketDataSent?.Invoke(this, new WebSocketDataSentEventArgs(webSocketClient.ID, e.BytesSent));
        }

        void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            var webSocketClient = _webSocketClientsManager.GetBySocket(e.Socket);
            if (webSocketClient == null)
                return;

            _webSocketClientsManager.Remove(webSocketClient);

            WebSocketDisconnected?.Invoke(this, new WebSocketDisconnectedEventArgs(webSocketClient.ID, e.Exception));
        }

        void DoHandshake(WebSocketClient client)
        {
            var response = _handshakeResponseGenerator.GetResponse(client.Buffer.GetData());
            if (response.Length > 0)
            {
                SendRawData(client.ID, response);

                client.HandshakeDone = true;
                client.Buffer.Clear();
            }
        }

        void ProcessMessage(WebSocketClient client)
        {
            var frame = client.Buffer.GetData();

            var deserializeResult = DeserializeResult.None;
            var frameType = FrameType.None;
            var parsedBytes = 0;
            var message = _framesManager.Deserialize(frame, out deserializeResult, out frameType, out parsedBytes);

            if (parsedBytes > 0)
            {
                var commandExecutor = _commandsExecutorFactory.Create(frameType);

                if (commandExecutor.Do(this, client.ID, message))
                    WebSocketDataReceived?.Invoke(this, new WebSocketDataReceivedEventArgs(client.ID, message));

                client.Buffer.Remove(parsedBytes);
            }
        }
    }
}
