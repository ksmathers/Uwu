using System;
using System.Collections.Generic;
using System.Text;

namespace UwuNet
{


    public delegate void OrchestrationMessageHandler(string msg, Options opts);

    public interface IMessaging
    {
        /// <summary>
        /// Sends a multicast message to all peers on the network
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="opts">LAN - confine messages to 1 hop, WAN - allow messages to propagate</param>
        void SendMessage(string msg, Options opts = Options.LAN);

        /// <summary>
        /// Sends a message to a specific peer on the network
        /// </summary>
        /// <param name="target">app@node</param>
        /// <param name="data"></param>
        void Send(string target, byte[] data);

        void Connect(string connectTo);

        /// <summary>
        /// 
        /// </summary>
        event OrchestrationMessageHandler MessageReceived;

        /// <summary>
        /// Delivers messages to the main thread
        /// </summary>
        event OrchestrationMessageHandler FormMessageReceived;
    }


}
