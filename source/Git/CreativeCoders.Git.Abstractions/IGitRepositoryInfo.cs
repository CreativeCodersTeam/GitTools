using System;
using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Provides information about a Git repository.
/// </summary>
[PublicAPI]
public interface IGitRepositoryInfo
{
    /// <summary>
    /// Gets the file system path of the repository.
    /// </summary>
    string? Path { get; }

    /// <summary>
    /// Gets the main branch type of the repository.
    /// </summary>
    GitMainBranch MainBranch { get; }

    /// <summary>
    /// Gets the URI of the primary remote.
    /// </summary>
    Uri RemoteUri { get; }
}