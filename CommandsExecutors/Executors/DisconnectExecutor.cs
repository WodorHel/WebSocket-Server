﻿namespace WS.CommandsExecutors.Executors
{
    class DisconnectExecutor : ICommandExecutor
    {
        public bool Do(WebSocketServer webSocketServer, string clientID, byte[] message)
        {
            webSocketServer.DisconnectClient(clientID);

            return false;
        }
    }
}