using System;
using System.Collections.Generic;
using System.Text;

namespace UwuNet
{
    public class OrchestrationMessage : BaseMessage
    {
        string body;

        /// <summary>
        /// A message type that contains a string
        /// </summary>
        public OrchestrationMessage()
            : base(MessageType.Orchestration, Options.WAN)
        {
            body = "";
        }

        /// <summary>
        /// The string value within the message
        /// </summary>
        public string Body {
            get { return body; }
            set { body = value; }
        }

        /// <summary>
        /// A message type that contains a string
        /// </summary>
        /// <param name="body">the string value</param>
        /// <param name="opts">options for message transports</param>
        public OrchestrationMessage(string body, Options opts = Options.WAN)
            : base(MessageType.Orchestration, opts)
        {
            //
            this.body = body;
        }

        public override void Transmit(Marshal tx)
        {
            base.TransmitHeader(tx);
            tx.WriteString(body);
        }

        public override void Receive(Marshal rx)
        {
            base.ReceiveHeader(rx);
            body = rx.ReadString();
        }
    }
}
