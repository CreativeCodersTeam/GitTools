using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs;

/// <summary>
/// Represents the set of changes between two trees in a Git repository, grouped by change kind.
/// </summary>
[PublicAPI]
public interface IGitTreeChanges : IEnumerable<IGitTreeEntryChanges>, IDisposable
{
    /// <summary>
    /// Gets the entries that were added.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Added { get; }

    /// <summary>
    /// Gets the entries that were deleted.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Deleted { get; }

    /// <summary>
    /// Gets the entries that were modified.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Modified { get; }

    /// <summary>
    /// Gets the entries whose type was changed.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> TypeChanged { get; }

    /// <summary>
    /// Gets the entries that were renamed.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Renamed { get; }

    /// <summary>
    /// Gets the entries that were copied.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Copied { get; }

    /// <summary>
    /// Gets the entries that were not modified.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Unmodified { get; }

    /// <summary>
    /// Gets the entries that are in a conflicted state.
    /// </summary>
    IEnumerable<IGitTreeEntryChanges> Conflicted { get; }

    /// <summary>
    /// Gets the total number of changed entries.
    /// </summary>
    int Count { get; }
}