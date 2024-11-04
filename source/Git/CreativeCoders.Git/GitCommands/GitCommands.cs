using CreativeCoders.Git.Abstractions.GitCommands;

namespace CreativeCoders.Git.GitCommands;

public class GitCommands : IGitCommands
{
    private readonly RepositoryContext _repositoryContext;

    internal GitCommands(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public IPullCommand CreatePullCommand()
    {
        return new PullCommand(_repositoryContext);
    }

    public IPushCommand CreatePushCommand()
    {
        return new PushCommand(_repositoryContext);
    }
}
