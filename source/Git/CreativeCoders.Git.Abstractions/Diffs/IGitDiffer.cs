using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs;

/// <summary>
/// Provides methods for comparing trees and producing diffs in a Git repository.
/// </summary>
[PublicAPI]
public interface IGitDiffer
{
    /// <summary>
    /// Compares the working directory against the index (staged area).
    /// </summary>
    /// <returns>The tree changes representing the differences.</returns>
    IGitTreeChanges Compare();

    /// <summary>
    /// Compares the working directory against the index (staged area).
    /// </summary>
    /// <param name="includeUntracked"><see langword="true"/> to include untracked files in the comparison; otherwise, <see langword="false"/>.</param>
    /// <returns>The tree changes representing the differences.</returns>
    IGitTreeChanges Compare(bool includeUntracked);

    /// <summary>
    /// Compares the working directory against the index for the specified paths.
    /// </summary>
    /// <param name="paths">The file paths to include in the comparison.</param>
    /// <returns>The tree changes representing the differences.</returns>
    IGitTreeChanges Compare(IEnumerable<string> paths);

    /// <summary>
    /// Compares the working directory against the index for the specified paths.
    /// </summary>
    /// <param name="paths">The file paths to include in the comparison.</param>
    /// <param name="includeUntracked"><see langword="true"/> to include untracked files in the comparison; otherwise, <see langword="false"/>.</param>
    /// <returns>The tree changes representing the differences.</returns>
    IGitTreeChanges Compare(IEnumerable<string> paths, bool includeUntracked);
}