using System;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using LibGit2Sharp;

namespace CreativeCoders.Git;

internal class GitRepositoryInfo : IGitRepositoryInfo
{
    public GitRepositoryInfo(IRepository repository)
    {
        Path = repository.Info.Path;

        MainBranch = GetMainBranch(repository);

        RemoteUri = new Uri(repository.Network.Remotes[GitRemotes.Origin].Url);
    }

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

    public string? Path { get; }

    public GitMainBranch MainBranch { get; }

    public Uri RemoteUri { get; }
}