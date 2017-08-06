using WS.CommandsExecutors.Executors;
using WS.Protocol.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.CommandsExecutors
{
    internal interface ICommandExecutorFactory
    {
        ICommandExecutor Create(FrameType type);
    }
}
