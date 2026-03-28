using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

/// <summary>
/// Represents filter criteria for querying the commit log.
/// </summary>
[PublicAPI]
public class GitCommitFilter
{
    /// <summary>
    /// Gets or sets a value that indicates whether only the first parent of merge commits should be followed.
    /// </summary>
    /// <value><see langword="true"/> to follow only the first parent; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</value>
    public bool FirstParentOnly { get; set; }

    /// <summary>
    /// Gets or sets the commit or reference from which reachable commits are included.
    /// </summary>
    public object? IncludeReachableFrom { get; set; }

    /// <summary>
    /// Gets or sets the commit or reference from which reachable commits are excluded.
    /// </summary>
    public object? ExcludeReachableFrom { get; set; }

    /// <summary>
    /// Gets or sets the sorting strategy for the returned commits.
    /// </summary>
    public GitCommitSortStrategies SortBy { get; set; }
}