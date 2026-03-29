using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

/// <summary>
/// Represents the queryable log of commits in a Git repository or branch.
/// </summary>
[PublicAPI]
public interface IGitCommitLog : IEnumerable<IGitCommit>
{
    /// <summary>
    /// Gets all commits prior to the specified date.
    /// </summary>
    /// <param name="olderThan">The date threshold; only commits older than this value are returned.</param>
    /// <returns>A sequence of commits older than <paramref name="olderThan"/>.</returns>
    IEnumerable<IGitCommit> GetCommitsPriorTo(DateTimeOffset olderThan);

    /// <summary>
    /// Queries the commit log using the specified filter.
    /// </summary>
    /// <param name="commitFilter">The filter criteria for selecting commits.</param>
    /// <returns>A sequence of commits matching the filter.</returns>
    IEnumerable<IGitCommit> QueryBy(GitCommitFilter commitFilter);
}