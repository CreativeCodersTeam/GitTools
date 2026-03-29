using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.Git;

/// <summary>
/// Provides metadata information about a Git repository such as path, main branch, and remote URI.
/// </summary>
/// <param name="repository">The underlying LibGit2Sharp repository.</param>
internal class GitRepositoryInfo(IRepository repository) : IGitRepositoryInfo
{
    private static GitMainBranch GetMainBranch(IRepository repository)
    {
        var mainBranch = repository.Branches[GitBranchNames.Remote.Main.CanonicalName];

        if (mainBranch != null)
        {
            return GitMainBranch.Main;
        }

        var masterBranch = repository.Branches[GitBranchNames.Remote.Master.CanonicalName];

        return masterBranch != null
            ? GitMainBranch.Master
            : GitMainBranch.Custom;
    }

    /// <inheritdoc />
    public string? Path { get; } = Ensure.NotNull(repository).Info.Path;

    /// <inheritdoc />
    public GitMainBranch MainBranch { get; } = GetMainBranch(repository);

    /// <inheritdoc />
    public Uri RemoteUri { get; } = new Uri(Ensure.NotNull(repository).Network.Remotes[GitRemotes.Origin].Url);
}
