using CreativeCoders.Git.Abstractions.GitCommands;

namespace CreativeCoders.Git.GitCommands;

/// <summary>
/// Provides a factory for creating Git command instances such as pull, push, and fetch tags.
/// </summary>
public class GitCommands : IGitCommands
{
    private readonly RepositoryContext _repositoryContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitCommands"/> class.
    /// </summary>
    /// <param name="repositoryContext">The repository context.</param>
    internal GitCommands(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    /// <inheritdoc />
    public IPullCommand CreatePullCommand()
    {
        return new PullCommand(_repositoryContext);
    }

    /// <inheritdoc />
    public IPushCommand CreatePushCommand()
    {
        return new PushCommand(_repositoryContext);
    }

    /// <inheritdoc />
    public IFetchTagsCommand CreateFetchTagsCommand()
    {
        return new FetchTagsCommand(_repositoryContext);
    }
}
