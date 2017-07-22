﻿using WS.SocketClients;
using WS.SocketServices.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.SocketServices
{
    class SendingDataService
    {
        public event EventHandler<DataSentEventArgs> SentData;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        public SendingDataService()
        {

        }

        public void SendData(Client client, byte[] data)
        {
            var clientSocket = client.Socket;

            try
            {
                clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(EndSendData), client);
            }
            catch(Exception ex)
            {
                Disconnected(this, new DisconnectedEventArgs(client, ex));
                return;
            }
        }

        void EndSendData(IAsyncResult ar)
        {
            var client = (Client)ar.AsyncState;
            var sentBytes = 0;

            try
            {
                sentBytes = client.Socket.EndSend(ar);
            }
            catch(Exception ex)
            {
                Disconnected(this, new DisconnectedEventArgs(client, ex));
                return;
            }

            SentData(this, new DataSentEventArgs(client, sentBytes));
        }
    }
}