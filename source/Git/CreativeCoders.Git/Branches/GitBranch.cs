using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Commits;

namespace CreativeCoders.Git.Branches;

/// <summary>
/// Represents a Git branch wrapping a LibGit2Sharp <see cref="Branch"/>.
/// </summary>
public class GitBranch : ComparableObject<GitBranch, IGitBranch>, IGitBranch
{
    private readonly Branch _branch;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitBranch"/> class.
    /// </summary>
    /// <param name="branch">The underlying LibGit2Sharp branch.</param>
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

    /// <inheritdoc />
    public ReferenceName Name { get; }

    /// <inheritdoc />
    public IGitCommit? Tip { get; }

    /// <inheritdoc />
    public bool IsRemote => _branch.IsRemote;

    /// <inheritdoc />
    public bool IsTracking => _branch.IsTracking;

    /// <inheritdoc />
    public IGitBranch? TrackedBranch { get; }

    /// <inheritdoc />
    public bool IsDetachedHead => Name.Canonical.Equals("(no branch)", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public IGitCommitLog? Commits { get; }

    /// <summary>
    /// Converts a <see cref="GitBranch"/> to a LibGit2Sharp <see cref="Branch"/>.
    /// </summary>
    /// <param name="branch">The Git branch to convert.</param>
    public static implicit operator Branch(GitBranch branch) => branch._branch;
}
