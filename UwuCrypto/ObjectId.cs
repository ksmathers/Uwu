using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Data;

namespace UwuCrypto
{
    public class ObjectId
    {
        public const string pkix = "1.3.6.1.5.5.7";

        // RFC 3280 5280
        const string kp = pkix + ".3";
        const string kp_serverAuth = kp + ".1";
        const string kp_clientAuth = kp + ".2";
        const string kp_codeSigning = kp + ".3";
        const string kp_emailProtection = kp + ".4";
        const string kp_timeStamping = kp + ".8";
        const string kp_OCSPSigning = kp + ".9";

        public const string joint_iso_ccitt = "2";
        public const string ce = joint_iso_ccitt + ".5" + ".29";

        //
        public static Oid kpServerAuth = Oid.FromOidValue(kp_serverAuth, OidGroup.EnhancedKeyUsage);
        public static Oid kpClientAuth = Oid.FromOidValue(kp_clientAuth, OidGroup.EnhancedKeyUsage);
        public static Oid kpCodeSigning = Oid.FromOidValue(kp_codeSigning, OidGroup.EnhancedKeyUsage);
        public static Oid kpEmailProtection = Oid.FromOidValue(kp_emailProtection, OidGroup.EnhancedKeyUsage);
        public static Oid kpTimeStamping = Oid.FromOidValue(kp_timeStamping, OidGroup.EnhancedKeyUsage);
        public static Oid kpOCSPSigning = Oid.FromOidValue(kp_OCSPSigning, OidGroup.EnhancedKeyUsage);

        public Oid id;
        public ObjectId(Oid _id)
        {
            id = _id;
        }
    }

    //public class Collection
    //{
    //    OidCollection set;

    //    public Collection()
    //    {
    //        set = new OidCollection();
    //        Add(ObjectId.kpClientAuth);
    //    }

    //    public void Add(ObjectId id)
    //    {
    //        set.Add(id.id);
    //    }

    //    public OidCollection Value {
    //        get { return set; }
    //    }
    //}
}
