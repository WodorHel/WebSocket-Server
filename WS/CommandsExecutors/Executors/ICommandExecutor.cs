namespace WS.CommandsExecutors.Executors
{
    internal interface ICommandExecutor
    {
        bool Do(WebSocketServer webSocketServer, string clientID, byte[] message);
    }
}
