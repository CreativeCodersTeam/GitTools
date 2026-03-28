namespace CreativeCoders.Git.Abstractions.GitCommands;

/// <summary>
/// Represents a command for fetching tags from a remote repository.
/// </summary>
public interface IFetchTagsCommand
{
    /// <summary>
    /// Executes the fetch tags command with the specified options.
    /// </summary>
    /// <param name="commandOptions">The options controlling the fetch tags behavior.</param>
    void Execute(FetchTagsCommandOptions commandOptions);
}
