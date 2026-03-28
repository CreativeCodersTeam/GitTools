using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.Git;

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

    public string? Path { get; } = Ensure.NotNull(repository).Info.Path;

    public GitMainBranch MainBranch { get; } = GetMainBranch(repository);

    public Uri RemoteUri { get; } = new Uri(Ensure.NotNull(repository).Network.Remotes[GitRemotes.Origin].Url);
}
