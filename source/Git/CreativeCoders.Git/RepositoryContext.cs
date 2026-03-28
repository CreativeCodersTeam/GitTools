using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

internal class RepositoryContext(
    DefaultGitRepository repository,
    Repository libGitRepository,
    ILibGitCaller libGitCaller,
    Func<Signature> getSignature,
    Func<CredentialsHandler> getCredentialsHandler,
    Func<CertificateCheckHandler?> certificateCheckHandler)
{
    private readonly Func<CertificateCheckHandler?> _certificateCheckHandler = Ensure.NotNull(certificateCheckHandler);

    private readonly Func<CredentialsHandler> _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler);

    private readonly Func<Signature> _getSignature = Ensure.NotNull(getSignature);

    public Signature GetSignature()
    {
        return _getSignature();
    }

    public CredentialsHandler GetCredentialsHandler()
    {
        return _getCredentialsHandler();
    }

    public CertificateCheckHandler? GetCertificateCheckHandler()
    {
        return _certificateCheckHandler();
    }

    public Repository LibGitRepository { get; } = Ensure.NotNull(libGitRepository);

    public ILibGitCaller LibGitCaller { get; } = Ensure.NotNull(libGitCaller);

    public DefaultGitRepository Repository { get; } = Ensure.NotNull(repository);
}
