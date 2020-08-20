using System;
using System.Collections.Generic;
using System.Text;

namespace UwuNet
{


    public delegate void GroupMessageHandler(string msg, Options opts);
    public delegate void StateChangedHandler(object sender, string newstate);

    public interface IMessaging
    {
        /// <summary>
        /// Sends a multicast message (GroupMessage) to all peers on the network
        /// </summary>
        /// <param name="msg">any string</param>
        /// <param name="opts">LAN - confine messages to 1 hop, WAN - allow messages to propagate</param>
        void SendMessage(string msg, Options opts = Options.LAN);

        /// <summary>
        /// Sends a subclassed instance of BaseMessage.   
        /// </summary>
        /// <param name="msg">the message</param>
        void SendMessage(BaseMessage msg);

        /// <summary>
        /// Connects ths IMessaging instance to other networked peers
        /// </summary>
        /// <param name="connectTo">syntax dependent on protocol</param>
        void Connect(string connectTo = null);

        /// <summary>
        /// Shut down messaging, typically attached to application shutdown.  If not called then the message service thread may be left
        /// hanging.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns the current connection state.  'CONNECTED' indicates a live connection.  Anything else indicates messaging will 
        /// fail.
        /// </summary>
        string State {
            get;
        }

        /// <summary>
        /// Delivery callback for all GroupMessages that are received.
        /// </summary>
        event GroupMessageHandler MessageReceived;

        /// <summary>
        /// Callback activated when the connection state is updated.
        /// </summary>
        event StateChangedHandler StateChanged;
    }


}
