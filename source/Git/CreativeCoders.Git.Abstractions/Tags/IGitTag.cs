using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Tags;

/// <summary>
/// Represents a Git tag pointing to a target object.
/// </summary>
[PublicAPI]
public interface IGitTag : IEquatable<IGitTag?>, IComparable<IGitTag>, INamedReference
{
    /// <summary>
    /// Gets the SHA hash of the target object.
    /// </summary>
    string TargetSha { get; }

    /// <summary>
    /// Peels the tag to its target commit, resolving annotated tags to the underlying commit.
    /// </summary>
    /// <returns>The target commit, or <see langword="null"/> if the tag does not point to a commit.</returns>
    IGitCommit? PeeledTargetCommit();

    /// <summary>
    /// Gets the commit that the tag directly points to.
    /// </summary>
    IGitCommit? TargetCommit { get; }
}
