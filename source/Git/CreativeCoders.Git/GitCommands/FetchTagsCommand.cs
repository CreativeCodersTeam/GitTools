using CreativeCoders.Git.Abstractions.GitCommands;

namespace CreativeCoders.Git.GitCommands;

/// <summary>
/// Implements the fetch tags command that fetches all tags from a remote repository.
/// </summary>
/// <param name="repositoryContext">The repository context.</param>
internal class FetchTagsCommand(RepositoryContext repositoryContext) : IFetchTagsCommand
{
    private readonly RepositoryContext _repositoryContext = Ensure.NotNull(repositoryContext);

    /// <inheritdoc />
    public void Execute(FetchTagsCommandOptions commandOptions)
    {
        var fetchOptions = new FetchOptions
        {
            Prune = commandOptions.Prune,
            TagFetchMode = TagFetchMode.All
        };

        Commands.Fetch(_repositoryContext.LibGitRepository, commandOptions.RemoteName, ["+refs/tags/*:refs/tags/*"],
            fetchOptions,
            "Fetch all tags");
    }
}
