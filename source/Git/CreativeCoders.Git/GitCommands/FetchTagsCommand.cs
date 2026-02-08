using CreativeCoders.Git.Abstractions.GitCommands;

namespace CreativeCoders.Git.GitCommands;

internal class FetchTagsCommand(RepositoryContext repositoryContext) : IFetchTagsCommand
{
    private readonly RepositoryContext _repositoryContext = Ensure.NotNull(repositoryContext);

    public void Execute(string remoteName = "origin")
    {
        var fetchOptions = new FetchOptions
        {
            Prune = true,
            TagFetchMode = TagFetchMode.All
        };

        Commands.Fetch(_repositoryContext.LibGitRepository, remoteName, ["+refs/tags/*:refs/tags/*"],
            fetchOptions,
            "Fetch all tags");
    }
}
