using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ITMSInstallerServer{
    class EphemeralECCCertificate {
        public X509Certificate2 Certificate { get; private set; }

        public string PfxPassword { get; private set; }

        public string PfxName { get; private set; }

        public EphemeralECCCertificate(IPAddress[] ips)
        {
            var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var csr = new CertificateRequest($"CN={Program.Name}", key, HashAlgorithmName.SHA256);
            csr.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            csr.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            csr.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(csr.PublicKey, false));
            csr.CertificateExtensions.Add(CreateSubjectAlternativeNameExtension(ips));
            Certificate = csr.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            // Windows platform needs to export the certificate, so generate PfxPassword and WindowsPlatformRelativePath here
            var t = DateTime.UtcNow.Ticks - DateTime.UnixEpoch.Ticks;
            PfxName = $"pfx_{t}.pfx";
            PfxPassword = Guid.NewGuid().ToString("N");
        }

        private static X509Extension CreateSubjectAlternativeNameExtension(IEnumerable<IPAddress> ips) {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddDnsName(Program.Name);
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            ips.ToList().ForEach(sanBuilder.AddIpAddress);
            return sanBuilder.Build();
        }
    }
}