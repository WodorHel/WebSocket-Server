using WS.Protocol.Frame;

namespace WS.CommandsExecutors.Executors
{
    class PingExecutor : ICommandExecutor
    {
        public bool Do(WebSocketServer webSocketServer, string clientID, byte[] message)
        {
            webSocketServer.SendData(clientID, message, FrameType.Pong);
            return false;
        }
    }
}
