using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Diagnostics;

namespace UwuCrypto
{
    public class SimpleCA
    {

        // BasicConstraints
        Dictionary<string,string> basicConstraints = new Dictionary<string, string>();

        X509Certificate2 rootCert;
        RSA rootKey;

        public X509Extension SetBasicConstraints(bool isCA, int maxPathLength=0)
        {
            bool critical = true;
            bool limitPathLength = false;
            if (maxPathLength > 0) {
                limitPathLength = true;
            }
            var v = new X509BasicConstraintsExtension(isCA, limitPathLength, maxPathLength, critical);
            return v;
        }

        public X509Extension SetEnhancedKeyUsage(
            bool digitalSignature = false,
            bool nonRepudiation = false,
            bool keyEncipherment = false,
            bool keyAgreement = false,
            bool keyCertSign = false,
            bool cRLSign = false,
            bool encipherOnly = false,
            bool decipherOnly = false)
        {
            string pkix = "1.3.6.1.5.5.7";
            string kp = $"{pkix}.3";
            string serverAuth = $"{kp}.1";
            
            var q = Oid.FromOidValue(serverAuth, OidGroup.EnhancedKeyUsage);
            var r = new OidCollection();
            r.Add(q);
            var v = new X509EnhancedKeyUsageExtension(r, false);
            return v;
        }


        public SimpleCA(string countryCode, string organization, string orgunit, string commonName) {
            var distinguishedName = $"C={countryCode};O={organization};OU={orgunit};CN={commonName}";
            //var distinguishedName = "C=US;O=Microsoft;OU=WGA;CN=TedSt";
            //CN={commonName}/OU={organizationUnit}/O={organization}/DC={domain}/STREET={address}/L={locality}/ST={state}/C={country}/DC={domainComponent}"

            var dn = new System.Security.Cryptography.X509Certificates.X500DistinguishedName(
                    distinguishedName
                );

            var key = System.Security.Cryptography.RSA.Create();
            key.KeySize = 4096;
            var hash = System.Security.Cryptography.HashAlgorithmName.SHA512;

            var req = new System.Security.Cryptography.X509Certificates.CertificateRequest(dn, key, hash, RSASignaturePadding.Pss);
            var q = Oid.FromFriendlyName("", OidGroup.EnhancedKeyUsage);
            Oid.FromOidValue("", OidGroup.EnhancedKeyUsage);
            var r = new OidCollection();
            r.Add(q);
            var v = new X509EnhancedKeyUsageExtension(r, false);
            req.CertificateExtensions.Add(SetEnhancedKeyUsage(keyCertSign: true));
            req.CertificateExtensions.Add(SetBasicConstraints(true));
            req.CertificateExtensions.Add(v);
            var now = DateTimeOffset.UtcNow;
            var startDate = now - TimeSpan.FromMinutes(5);
            var endDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(365.25 * 10);
            rootCert = req.CreateSelfSigned(startDate, endDate);
            rootKey = key;

            var rootKeyChain = rootCert.Export(X509ContentType.Pkcs12);

        }

        public SimpleCA(byte[] buf)
        {
            rootCert = new X509Certificate2();
            rootCert.Import(buf);
            rootKey = rootCert.PrivateKey as RSA;
        }

        public void Save(string fname)
        {
            if (!fname.EndsWith(".p12")) fname += ".p12";
            var fsave = System.IO.File.OpenWrite(fname);
            fsave.Write(rootCert.Export(X509ContentType.Pkcs12));
            fsave.Close();
        }

        public static SimpleCA Load(string fname)
        {
            if (!fname.EndsWith(".p12")) fname += ".p12";
            var fload = System.IO.File.OpenRead(fname);
            byte[] buf = new byte[fload.Length];
            fload.Read(buf, 0, buf.Length);
            return new SimpleCA(buf);
        }

        public X509Certificate2 RootCert {
            get { return rootCert; }
        }
    }
}
