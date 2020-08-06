using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace UwuNet
{
    /// <summary>
    /// Common basis for all messages sent through UwuNet.   Includes Options (for propagation control) and MessageType (for the message factory
    /// to use when deserializing).  The SYNC, and LEN fields precede each message but are not part of the message directly.
    /// </summary>
    public class BaseMessage
    {
        MessageType mtype;
        Options opts;

        public BaseMessage(MessageType mtype, Options opts)
        {
            this.mtype = mtype;
            this.opts = opts;
        }

        public bool IsLan {
            get { return (opts & Options.LAN) != 0; }
            set {
                if (value) { opts |= Options.LAN; } else { opts &= ~Options.LAN; }
            }
        }

        public bool IsWan {
            get { return (opts & Options.WAN) != 0; }
            set {
                if (value) { opts |= Options.WAN; } else { opts &= ~Options.WAN; }
            }
        }

        public Options Options {
            get { return opts; }
            set { opts = value; }
        }

        /// <summary>
        /// Helper method for subcleasses Transmit() implementations
        /// </summary>
        /// <param name="tx"></param>
        public void TransmitHeader(Marshal tx)
        {
            tx.WriteInt32((int)mtype);
            tx.WriteInt32((int)opts);
        }

        /// <summary>
        /// Helper method for subclasses Receive() implementations
        /// </summary>
        /// <param name="rx"></param>
        public void ReceiveHeader(Marshal rx)
        {
            var _mtype = (MessageType)rx.ReadInt32();
            Debug.Assert(_mtype == mtype);
            opts = (Options)rx.ReadInt32();
        }

        /// <summary>
        /// Serializes intot he Marshalled IOBuffer from a specific subclass of BaseMessage
        /// </summary>
        /// <param name="tx"></param>
        public virtual void Transmit(Marshal tx)
        {
            throw new SerializationException("Unable to serialize BaseMessage");
        }

        /// <summary>
        /// Deserializes from the Marshalled IOBuffer into a specific subclass of BaseMessage
        /// </summary>
        /// <param name="rx"></param>
        public virtual void Receive(Marshal rx)
        {
            throw new SerializationException("Unable to serialize BaseMessage");
        }
    }
}
