namespace CreativeCoders.Git.Abstractions.Certs;

public delegate bool HostCertificateCheckHandler(IGitRepository repository, CertificateCheckArgs args);
