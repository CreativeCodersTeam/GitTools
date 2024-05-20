using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

[PublicAPI]
public static class GitRepositoryFetchExtensions
{
    public static void FetchFromOrigin(this IGitRepository gitRepository, GitFetchOptions fetchOptions)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, fetchOptions);
    }

    public static void FetchAllTags(this IGitRepository gitRepository, string remoteName)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(remoteName, new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    public static void FetchAllTagsFromOrigin(this IGitRepository gitRepository)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    public static void FetchPrune(this IGitRepository gitRepository, string remoteName)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(remoteName, new GitFetchOptions { Prune = true });
    }

    public static void FetchPruneFromOrigin(this IGitRepository gitRepository)
    {
        Ensure.NotNull(gitRepository)
            .Fetch(GitRemotes.Origin, new GitFetchOptions { Prune = true });
    }
}
