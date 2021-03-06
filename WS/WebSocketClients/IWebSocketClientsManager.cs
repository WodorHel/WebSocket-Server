﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WS.WebSocketClients
{
    internal interface IWebSocketClientsManager
    {
        void Add(WebSocketClient client);
        void Remove(WebSocketClient client);
        WebSocketClient GetByID(string id);
        WebSocketClient GetBySocket(Socket socket);
        bool Exists(string id);
    }
}
