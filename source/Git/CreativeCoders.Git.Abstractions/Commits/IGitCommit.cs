using System;
using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

/// <summary>
/// Represents a Git commit object.
/// </summary>
[PublicAPI]
public interface IGitCommit : IEquatable<IGitCommit>, IComparable<IGitCommit>, IGitObject
{
    /// <summary>
    /// Gets the parent commits of this commit.
    /// </summary>
    IEnumerable<IGitCommit> Parents { get; }

    /// <summary>
    /// Gets the author signature of the commit.
    /// </summary>
    IGitSignature Author { get; }

    /// <summary>
    /// Gets the committer signature of the commit.
    /// </summary>
    IGitSignature Committer { get; }

    /// <summary>
    /// Gets the commit message.
    /// </summary>
    string Message { get; }
}