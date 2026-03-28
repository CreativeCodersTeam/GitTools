using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Certs;

/// <summary>
/// Provides the arguments for a certificate check callback during a Git remote operation.
/// </summary>
[PublicAPI]
public class CertificateCheckArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateCheckArgs"/> class.
    /// </summary>
    /// <param name="certificate">The X.509 certificate, or <see langword="null"/> if not available.</param>
    /// <param name="sshCertificate">The SSH certificate, or <see langword="null"/> if the connection is not SSH.</param>
    /// <param name="host">The host name of the remote server.</param>
    public CertificateCheckArgs(X509Certificate? certificate, SshCertificate? sshCertificate, string host)
    {
        Certificate = certificate;
        Host = host;
        SshCertificate = sshCertificate;
    }

    /// <summary>
    /// Gets the X.509 certificate presented by the remote server.
    /// </summary>
    public X509Certificate? Certificate { get; }

    /// <summary>
    /// Gets a value that indicates whether the connection uses SSH.
    /// </summary>
    /// <value><see langword="true"/> if the connection uses SSH; otherwise, <see langword="false"/>.</value>
    public bool IsSsh => SshCertificate != null;

    /// <summary>
    /// Gets the SSH certificate presented by the remote server.
    /// </summary>
    public SshCertificate? SshCertificate { get; }

    /// <summary>
    /// Gets or sets a value that indicates whether the certificate is valid.
    /// </summary>
    /// <value><see langword="true"/> if the certificate is valid; otherwise, <see langword="false"/>.</value>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets the host name of the remote server.
    /// </summary>
    public string Host { get; }
}
