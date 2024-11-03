using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

internal class RepositoryContext
{
    private readonly Func<CertificateCheckHandler?> _certificateCheckHandler;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly Func<Signature> _getSignature;

    public RepositoryContext(DefaultGitRepository repository, Repository libGitRepository, ILibGitCaller libGitCaller,
        Func<Signature> getSignature, Func<CredentialsHandler> getCredentialsHandler,
        Func<CertificateCheckHandler?> certificateCheckHandler)
    {
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler);
        _getSignature = Ensure.NotNull(getSignature);

        LibGitRepository = Ensure.NotNull(libGitRepository);
        LibGitCaller = Ensure.NotNull(libGitCaller);
        Repository = Ensure.NotNull(repository);

        _certificateCheckHandler = certificateCheckHandler;
    }

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

    public Repository LibGitRepository { get; }

    public ILibGitCaller LibGitCaller { get; }

    public DefaultGitRepository Repository { get; }
}
