using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.WebSocketEventArguments
{
    public class WebSocketConnectedEventArgs : EventArgs
    {
        public String ClientID { get; private set; }

        public WebSocketConnectedEventArgs(String clientID)
        {
            this.ClientID = clientID;
        }
    }
}
