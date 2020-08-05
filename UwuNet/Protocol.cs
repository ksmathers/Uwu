using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace UwuNet
{
    public class Protocol<T> where T:IMessaging, new()
    {
        public static string prefix;

        public static IMessaging Create() {
            return new T();
        }
        public static void Register(Registry r)
        {
            prefix = typeof(T).Name.ToLowerInvariant();
            r.Add(prefix, Create);
        }
    }

}
