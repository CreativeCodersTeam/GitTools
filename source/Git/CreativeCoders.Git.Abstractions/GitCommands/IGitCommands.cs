namespace CreativeCoders.Git.Abstractions.GitCommands;

/// <summary>
/// Provides factory methods for creating Git command instances.
/// </summary>
public interface IGitCommands
{
    /// <summary>
    /// Creates a new pull command.
    /// </summary>
    /// <returns>A new <see cref="IPullCommand"/> instance.</returns>
    IPullCommand CreatePullCommand();

    /// <summary>
    /// Creates a new push command.
    /// </summary>
    /// <returns>A new <see cref="IPushCommand"/> instance.</returns>
    IPushCommand CreatePushCommand();

    /// <summary>
    /// Creates a new fetch tags command.
    /// </summary>
    /// <returns>A new <see cref="IFetchTagsCommand"/> instance.</returns>
    IFetchTagsCommand CreateFetchTagsCommand();
}
