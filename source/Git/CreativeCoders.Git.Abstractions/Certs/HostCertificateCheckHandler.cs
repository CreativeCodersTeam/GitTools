namespace CreativeCoders.Git.Abstractions.Certs;

/// <summary>
/// Represents the handler invoked to validate a server certificate during a Git remote operation.
/// </summary>
/// <param name="repository">The repository performing the remote operation.</param>
/// <param name="args">The certificate check arguments containing the certificate details.</param>
/// <returns><see langword="true"/> if the certificate is accepted; otherwise, <see langword="false"/>.</returns>
public delegate bool HostCertificateCheckHandler(IGitRepository repository, CertificateCheckArgs args);
