using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.RefSpecs;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Remotes;

/// <summary>
/// Represents a configured Git remote.
/// </summary>
[PublicAPI]
public interface IGitRemote
{
    /// <summary>
    /// Gets the name of the remote.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the fetch URL of the remote.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Gets the push URL of the remote.
    /// </summary>
    string PushUrl { get; }

    /// <summary>
    /// Gets all refspecs configured for the remote.
    /// </summary>
    IEnumerable<IGitRefSpec> RefSpecs { get; }

    /// <summary>
    /// Gets the fetch refspecs configured for the remote.
    /// </summary>
    IEnumerable<IGitRefSpec> FetchRefSpecs { get; }

    /// <summary>
    /// Gets the push refspecs configured for the remote.
    /// </summary>
    IEnumerable<IGitRefSpec> PushRefSpecs { get; }
}