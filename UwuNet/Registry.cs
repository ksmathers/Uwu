using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace UwuNet
{
    public delegate IMessaging BuildMessaging();
    public class Registry
    {
        Dictionary<string, BuildMessaging> protocolTable;

        public Registry()
        {
            protocolTable = new Dictionary<string, BuildMessaging>();
            MCast.MCast.Register(this);
        }

        public void Add(string prefix, BuildMessaging creator)
        {
            protocolTable.Add(prefix, creator);
        }

        public IMessaging Connect(string connectString)
        {
            string[] args = connectString.Split(new char[] { ':' }, 2);
            string proto = args[0];
            string connectTo = args[1];
            IMessaging msgs = null;
            if (protocolTable.ContainsKey(proto)) {
                msgs = protocolTable[proto]();
                msgs.Connect(connectTo);
            } else {
                throw new ArgumentOutOfRangeException($"Invalid protocol in connection string {connectString}");       
            }
            return msgs;
        }
    }
}
