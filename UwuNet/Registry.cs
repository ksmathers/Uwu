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

        public (string, string) ParseConnectionString(string connectString)
        {
            string[] args = connectString.Split(new char[] { ':' }, 2);
            string proto = args[0];
            string connectTo = args[1];
            return (proto, connectTo);
        }

        public IMessaging Create(string proto)
        {
            IMessaging msgs = null;
            if (protocolTable.ContainsKey(proto)) {
                msgs = protocolTable[proto]();
            } else {
                throw new ArgumentOutOfRangeException($"Invalid protocol in connection string {proto}");       
            }
            return msgs;
        }
    }
}
