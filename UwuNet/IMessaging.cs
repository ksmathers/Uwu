using System;
using System.Collections.Generic;
using System.Text;

namespace UwuNet
{


    public delegate void OrchestrationMessageHandler(string msg, Options opts);
    public delegate void StateChangedHandler(object sender, string newstate);

    public interface IMessaging
    {
        /// <summary>
        /// Sends a multicast message (OrchestrationMessage) to all peers on the network
        /// </summary>
        /// <param name="msg">any string</param>
        /// <param name="opts">LAN - confine messages to 1 hop, WAN - allow messages to propagate</param>
        void SendMessage(string msg, Options opts = Options.LAN);

        void SendMessage(BaseMessage msg);

        void Connect(string connectTo = null);

        void Stop();

        string State {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        event OrchestrationMessageHandler MessageReceived;
        event StateChangedHandler StateChanged;
    }


}
