using CreativeCoders.Git.Abstractions.GitCommands;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public class GitCommands : IGitCommands
{
    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly Func<Signature> _getSignature;

    private readonly ILibGitCaller _libGitCaller;
    private readonly DefaultGitRepository _repository;

    internal GitCommands(DefaultGitRepository repository, Func<CredentialsHandler> getCredentialsHandler,
        Func<Signature> getSignature, ILibGitCaller libGitCaller)
    {
        _repository = Ensure.NotNull(repository);
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler);
        _getSignature = Ensure.NotNull(getSignature);
        _libGitCaller = Ensure.NotNull(libGitCaller);
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
