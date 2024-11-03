using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Certs;

[PublicAPI]
public class CertificateCheckArgs
{
    public CertificateCheckArgs(X509Certificate? certificate, SshCertificate? sshCertificate, string host)
    {
        Certificate = certificate;
        Host = host;
        SshCertificate = sshCertificate;
    }

    public X509Certificate? Certificate { get; }

    public bool IsSsh => SshCertificate != null;

    public SshCertificate? SshCertificate { get; }

    public bool IsValid { get; set; }

    public string Host { get; }
}
