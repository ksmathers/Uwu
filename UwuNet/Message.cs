using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace UwuNet
{
    [Flags]
    public enum Options
    {
        INVALID = 0,
        LAN = 1,
        WAN = 2
    }

    public enum MessageType
    {
        Orchestration,
    }

    public class BaseMessage
    {
        MessageType mtype;
        Options opts;
        int len;

        public BaseMessage(MessageType mtype, Options opts)
        {
            this.mtype = mtype;
            this.opts = opts;
        }

        public bool IsLan {
            get { return (opts & Options.LAN) != 0; }
            set {
                if (value) { opts |= Options.LAN; }
                else { opts &= ~Options.LAN; }
            }
        }

        public bool IsWan {
            get { return (opts & Options.WAN) != 0; }
            set {
                if (value) { opts |= Options.WAN; }
                else { opts &= ~Options.WAN; }
            }
        }

        public Options Options {
            get { return opts; }
            set { opts = value; }
        }

        public void TransmitHeader(Marshal tx)
        {
            tx.WriteInt32((int)mtype);
            tx.WriteInt32((int)opts);
        }

        public void ReceiveHeader(Marshal rx)
        {
            mtype = (MessageType)rx.ReadInt32();
            opts = (Options)rx.ReadInt32();
        }

        public virtual void Transmit(Marshal tx)
        {
            throw new SerializationException("Unable to serialize BaseMessage");
        }

        public virtual void Receive(Marshal rx)
        {
            throw new SerializationException("Unable to serialize BaseMessage");
        }
    }

    public class OrchestrationMessage : BaseMessage
    {
        string body;

        public OrchestrationMessage()
            : base(MessageType.Orchestration, Options.WAN)
        {
            body = "";
        }

        public string Body {
            get { return body; }
            set { body = value; }
        }

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

    public static class Message
    {
        public static void Transmit(IOBuffer iobuf, BaseMessage msg)
        {
            Marshal tx = new Marshal(iobuf);
            tx.WriteInt32(0);
            msg.Transmit(tx);
            int len = iobuf.Length;
        }

        public static BaseMessage Receive(IOBuffer iobuf)
        {
            Marshal rx = new Marshal(iobuf);
            int mlen = rx.PeekUInt32();
            BaseMessage msg = null;
            if (iobuf.Length >= mlen) {
                rx.ReadInt32();
                MessageType mtype = (MessageType)rx.PeekInt32(0);
                switch (mtype) {
                    case MessageType.Orchestration:
                        msg = new OrchestrationMessage();
                        msg.Receive(rx);
                        break;
                }
            }
            return msg;
        }
    }
        
}
