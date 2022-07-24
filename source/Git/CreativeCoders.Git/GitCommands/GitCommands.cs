using CreativeCoders.Git.Abstractions.GitCommands;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public class GitCommands : IGitCommands
{
    private readonly Repository _repository;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly Func<Signature> _getSignature;

    public GitCommands(Repository repository, Func<CredentialsHandler> getCredentialsHandler,
        Func<Signature> getSignature)
    {
        _repository = Ensure.NotNull(repository, nameof(repository));
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _getSignature = Ensure.NotNull(getSignature, nameof(getSignature));
    }

    public IPullCommand CreatePullCommand()
    {
        return new PullCommand(_repository, _getCredentialsHandler, _getSignature);
    }
}
