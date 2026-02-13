using CreativeCoders.Git.Abstractions.GitCommands;

namespace CreativeCoders.Git.GitCommands;

internal class FetchTagsCommand(RepositoryContext repositoryContext) : IFetchTagsCommand
{
    private readonly RepositoryContext _repositoryContext = Ensure.NotNull(repositoryContext);

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
