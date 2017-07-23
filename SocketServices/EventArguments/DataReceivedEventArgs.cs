﻿using WS.SocketClients;
using System;

namespace WS.SocketServices.EventArguments
{
    class DataReceivedEventArgs : EventArgs
    {
        public Client Client { get; private set; }
        public int BytesReceived { get; private set; }

        public DataReceivedEventArgs(Client client, int bytesReceived)
        {
            Client = client;
            BytesReceived = bytesReceived;
        }
    }
}
