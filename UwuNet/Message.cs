using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
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
            var _mtype = (MessageType)rx.ReadInt32();
            Debug.Assert(_mtype == mtype);
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
        const uint SYNC = 0xBA5EBEA7;
        public static void Transmit(IOBuffer iobuf, BaseMessage msg)
        {
            Marshal tx = new Marshal(iobuf);
            tx.WriteUInt32(SYNC);
            int waddr = iobuf.WriteOffset;
            int msglen = 0;
            tx.WriteInt32(msglen);
            msg.Transmit(tx);
            msglen = iobuf.Length;
            tx.PokeInt32(waddr, msglen);
        }

        public static BaseMessage Receive(IOBuffer iobuf)
        {
            if (iobuf.Length < 12) return null;

            Marshal rx = new Marshal(iobuf);
            uint sync = (uint)rx.PeekInt32(0);
            int mlen = rx.PeekInt32(4);
            MessageType mtype = (MessageType)rx.PeekInt32(8);
            if (sync != SYNC) {
                throw new ProtocolViolationException("Bad sync");
            }
            BaseMessage msg = null;
            if (iobuf.Length >= mlen) {
                int _sync = rx.ReadInt32();
                int _mlen = rx.ReadInt32();
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
