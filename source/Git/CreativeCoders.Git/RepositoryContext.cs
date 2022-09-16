using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

public class RepositoryContext
{
    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly Func<Signature> _getSignature;

    public RepositoryContext(Repository repository, ILibGitCaller libGitCaller,
        Func<Signature> getSignature, Func<CredentialsHandler> getCredentialsHandler)
    {
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _getSignature = Ensure.NotNull(getSignature, nameof(getSignature));

        Repository = repository;
        LibGitCaller = libGitCaller;
    }

    public Signature GetSignature()
    {
        return _getSignature();
    }

    public CredentialsHandler GetCredentialsHandler()
    {
        return _getCredentialsHandler();
    }

    public Repository Repository { get; }

    public ILibGitCaller LibGitCaller { get; }
}
