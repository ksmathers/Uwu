using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace UwuNet
{
    /// <summary>
    /// Base class for UwuNet protocols with static registration and static virtual constructor methods
    /// </summary>
    /// <typeparam name="T">protocol implementation subclass</typeparam>
    public class Protocol<T> where T:IMessaging, new()
    {
        
        public static string prefix;

        /// <summary>
        /// Implementation of the Registry.MessagingCreator delegate
        /// </summary>
        /// <returns></returns>
        static IMessaging Create() {
            return new T();
        }

        /// <summary>
        /// Protocol registration initialization method, called from a derived class, for example:
        ///    MCast.Register(registry)
        /// </summary>
        /// <param name="r"></param>
        public static void Register(Registry r)
        {
            prefix = typeof(T).Name.ToLowerInvariant();
            r.Add(prefix, Create);
        }
    }

}
