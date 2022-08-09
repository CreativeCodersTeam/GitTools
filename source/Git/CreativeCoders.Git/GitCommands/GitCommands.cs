using CreativeCoders.Git.Abstractions.GitCommands;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public class GitCommands : IGitCommands
{
    private readonly Repository _repository;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly Func<Signature> _getSignature;

    private readonly ILibGitCaller _libGitCaller;

    public GitCommands(Repository repository, Func<CredentialsHandler> getCredentialsHandler,
        Func<Signature> getSignature, ILibGitCaller libGitCaller)
    {
        _repository = Ensure.NotNull(repository, nameof(repository));
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _getSignature = Ensure.NotNull(getSignature, nameof(getSignature));
        _libGitCaller = Ensure.NotNull(libGitCaller, nameof(libGitCaller));
    }

    public IPullCommand CreatePullCommand()
    {
        return new PullCommand(_repository, _getCredentialsHandler, _getSignature, _libGitCaller);
    }

    public IPushCommand CreatePushCommand()
    {
        return new PushCommand(_repository, _getCredentialsHandler, _libGitCaller);
    }
}
