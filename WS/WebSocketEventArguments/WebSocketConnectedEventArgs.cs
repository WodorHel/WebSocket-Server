﻿using System;

namespace WS.WebSocketEventArguments
{
    public class WebSocketConnectedEventArgs : EventArgs
    {
        public string ClientID { get; private set; }

        public WebSocketConnectedEventArgs(string clientID)
        {
            ClientID = clientID;
        }
    }
}
