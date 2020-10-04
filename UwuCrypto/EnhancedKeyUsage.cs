using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UwuCrypto
{
    public class EnhancedKeyUsage
    {
        public bool ClientAuth { get; set; }

        public OidCollection ToOoid()
        {
            var result = new OidCollection();
            if (ClientAuth) result.Add(ObjectId.kpClientAuth);
            return result;
        }
    }
}
