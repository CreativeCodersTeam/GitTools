using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Provides extension methods for fetch operations on <see cref="IGitRepository"/>.
/// </summary>
[PublicAPI]
public static class GitRepositoryFetchExtensions
{
    /// <summary>
    /// Fetches changes from the origin remote using the specified options.
    /// </summary>
    /// <param name="gitRepository">The repository to fetch into.</param>
    /// <param name="fetchOptions">The options controlling fetch behavior.</param>
    public static void FetchFromOrigin(this IGitRepository gitRepository, GitFetchOptions fetchOptions)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, fetchOptions);
    }

    /// <summary>
    /// Fetches all tags from the specified remote.
    /// </summary>
    /// <param name="gitRepository">The repository to fetch into.</param>
    /// <param name="remoteName">The name of the remote to fetch tags from.</param>
    public static void FetchAllTags(this IGitRepository gitRepository, string remoteName)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(remoteName, new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    /// <summary>
    /// Fetches all tags from the origin remote.
    /// </summary>
    /// <param name="gitRepository">The repository to fetch into.</param>
    public static void FetchAllTagsFromOrigin(this IGitRepository gitRepository)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    /// <summary>
    /// Fetches from the specified remote with pruning enabled, removing stale remote-tracking references.
    /// </summary>
    /// <param name="gitRepository">The repository to fetch into.</param>
    /// <param name="remoteName">The name of the remote to fetch from.</param>
    public static void FetchPrune(this IGitRepository gitRepository, string remoteName)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(remoteName, new GitFetchOptions { Prune = true });
    }

    /// <summary>
    /// Fetches from the origin remote with pruning enabled, removing stale remote-tracking references.
    /// </summary>
    /// <param name="gitRepository">The repository to fetch into.</param>
    public static void FetchPruneFromOrigin(this IGitRepository gitRepository)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, new GitFetchOptions { Prune = true });
    }
}
