using System;
using System.Net.Sockets;

namespace WS.SocketServices.EventArguments
{
    internal class ConnectedEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }

        public ConnectedEventArgs(Socket socket)
        {
            Socket = socket;
        }
    }
}
