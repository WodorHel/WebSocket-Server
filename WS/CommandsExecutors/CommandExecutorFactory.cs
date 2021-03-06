﻿using WS.CommandsExecutors.Executors;
using WS.Protocol.Frame;

namespace WS.CommandsExecutors
{
    internal class CommandExecutorFactory : ICommandExecutorFactory
    {
        public ICommandExecutor Create(FrameType type)
        {
            switch (type)
            {
                case FrameType.Message: return new MessageExecutor();
                case FrameType.Ping: return new PingExecutor();
                case FrameType.Pong: return new PongExecutor();
                case FrameType.Disconnect: return new DisconnectExecutor();
            }

            return null;
        }
    }
}
