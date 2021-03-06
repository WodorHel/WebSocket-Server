﻿using System;
using System.Net;

namespace WS.WebSocketClients
{
    public class ClientInfo
    {
        public string ClientID { get; set; }
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public DateTime JoinTime { get; set; }
    }
}
