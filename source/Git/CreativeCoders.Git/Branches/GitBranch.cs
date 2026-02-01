using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Commits;

namespace CreativeCoders.Git.Branches;

public class GitBranch : ComparableObject<GitBranch, IGitBranch>, IGitBranch
{
    private readonly Branch _branch;

    internal GitBranch(Branch branch)
    {
        _branch = Ensure.NotNull(branch);

        Name = new ReferenceName(branch.CanonicalName);
        Tip = GitCommit.From(branch.Tip);
        Commits = GitCommitLog.From(branch.Commits);
        TrackedBranch = From(branch.TrackedBranch);
    }

    static GitBranch() => InitComparableObject(x => x.Name.Canonical);

    internal static GitBranch? From(Branch? branch)
    {
        return branch == null
            ? null
            : new GitBranch(branch);
    }

    public ReferenceName Name { get; }

    public IGitCommit? Tip { get; }

    public bool IsRemote => _branch.IsRemote;

    public bool IsTracking => _branch.IsTracking;

    public IGitBranch? TrackedBranch { get; }

    public bool IsDetachedHead => Name.Canonical.Equals("(no branch)", StringComparison.OrdinalIgnoreCase);

    public IGitCommitLog? Commits { get; }

    public static implicit operator Branch(GitBranch branch) => branch._branch;
}
