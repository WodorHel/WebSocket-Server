﻿using WS.SocketClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.SocketServices.EventArguments
{
    class DataSentEventArgs : EventArgs
    {
        public Client Client { get; private set; }
        public int BytesSent { get; private set; }

        public DataSentEventArgs(Client client, int bytesSent)
        {
            this.Client = client;
            this.BytesSent = bytesSent;
        }
    }
}
