using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Remotes;

/// <summary>
/// Represents the collection of remotes configured for a Git repository.
/// </summary>
[PublicAPI]
public interface IGitRemoteCollection : IEnumerable<IGitRemote>
{
    /// <summary>
    /// Gets the remote with the specified name.
    /// </summary>
    /// <param name="name">The name of the remote.</param>
    /// <returns>The matching remote, or <see langword="null"/> if not found.</returns>
    IGitRemote? this[string name] { get; }

    //void Remove(string remoteName);

    //void Update(string remoteName, string refSpec);
}