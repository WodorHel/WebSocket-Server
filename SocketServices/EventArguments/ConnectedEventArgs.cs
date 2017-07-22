﻿using WS.SocketClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WS.SocketServices.EventArguments
{
    class ConnectedEventArgs : EventArgs
    {
        public Socket ClientSocket { get; private set; }

        public ConnectedEventArgs(Socket clientSocket)
        {
            this.ClientSocket = clientSocket;
        }
    }
}
