using System;
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

    /// <summary>
    /// Gets or sets the earliest committer date for included commits.
    /// Only commits with a committer date greater than or equal to this value are included.
    /// </summary>
    /// <value>A <see cref="DateTimeOffset"/> representing the start of the date range, or <see langword="null"/> to apply no lower bound.</value>
    public DateTimeOffset? After { get; set; }

    /// <summary>
    /// Gets or sets the latest committer date for included commits.
    /// Only commits with a committer date less than or equal to this value are included.
    /// </summary>
    /// <value>A <see cref="DateTimeOffset"/> representing the end of the date range, or <see langword="null"/> to apply no upper bound.</value>
    public DateTimeOffset? Before { get; set; }

    /// <summary>
    /// Gets or sets a filter for the author's email address.
    /// The matching behavior depends on <see cref="AuthorEmailExactMatch"/>.
    /// </summary>
    /// <value>A partial or full email address string, or <see langword="null"/> to apply no author filter.</value>
    public string? AuthorEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="AuthorEmail"/> must match the full email address exactly.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to require an exact (case-insensitive) match;
    /// <see langword="false"/> (the default) to match when the author email contains the specified value.
    /// </value>
    public bool AuthorEmailExactMatch { get; set; }

    /// <summary>
    /// Gets or sets a regular expression pattern applied to the commit message.
    /// Only commits whose message matches the pattern are included.
    /// </summary>
    /// <value>A regular expression pattern string, or <see langword="null"/> to apply no message filter.</value>
    public string? MessagePattern { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of commits to return.
    /// </summary>
    /// <value>The maximum number of commits, or <c>0</c> (the default) to return all matching commits.</value>
    public int MaxCount { get; set; }
}