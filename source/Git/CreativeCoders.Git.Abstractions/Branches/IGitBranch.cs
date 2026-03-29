using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Branches;

/// <summary>
/// Represents a Git branch.
/// </summary>
[PublicAPI]
public interface IGitBranch : IEquatable<IGitBranch>, IComparable<IGitBranch>, INamedReference
{
    /// <summary>
    /// Gets the commit at the tip of the branch.
    /// </summary>
    IGitCommit? Tip { get; }

    /// <summary>
    /// Gets a value that indicates whether this is a remote-tracking branch.
    /// </summary>
    /// <value><see langword="true"/> if this is a remote-tracking branch; otherwise, <see langword="false"/>.</value>
    bool IsRemote { get; }

    /// <summary>
    /// Gets a value that indicates whether this branch is tracking a remote branch.
    /// </summary>
    /// <value><see langword="true"/> if this branch tracks a remote branch; otherwise, <see langword="false"/>.</value>
    bool IsTracking { get; }

    /// <summary>
    /// Gets the remote-tracking branch that this branch is tracking.
    /// </summary>
    IGitBranch? TrackedBranch { get; }

    /// <summary>
    /// Gets a value that indicates whether the branch represents a detached HEAD.
    /// </summary>
    /// <value><see langword="true"/> if HEAD is detached; otherwise, <see langword="false"/>.</value>
    bool IsDetachedHead { get; }

    /// <summary>
    /// Gets the commit log for this branch.
    /// </summary>
    IGitCommitLog? Commits { get; }
}