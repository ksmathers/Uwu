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
    /// <summary>
    /// Used for message propagation.  Messages that have crossed a WAN link are marked LAN so that they don't echo back 
    /// to the originating hub.
    /// </summary>
    [Flags]
    public enum Options
    {
        INVALID = 0,
        LAN = 1,
        WAN = 2
    }

    /// <summary>
    /// List of valid message types
    /// </summary>
    public enum MessageType
    {
        Orchestration,          // String with peer broadcast
    }





    /// <summary>
    /// A static class that acts as a message factory for receiving messages 
    /// </summary>
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
