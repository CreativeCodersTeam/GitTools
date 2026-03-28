namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Represents options for a Git fetch operation.
/// </summary>
public class GitFetchOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether remote-tracking references that no longer exist on the remote should be removed.
    /// </summary>
    /// <value><see langword="true"/> to prune stale remote-tracking references; otherwise, <see langword="false"/>. The default is <see langword="null"/>, which uses the configured default.</value>
    public bool? Prune { get; set; }

    /// <summary>
    /// Gets or sets the tag fetch mode controlling which tags are downloaded.
    /// </summary>
    public GitTagFetchMode? TagFetchMode { get; set; }

    /// <summary>
    /// Gets or sets custom headers to include in the fetch request.
    /// </summary>
    public string[]? CustomHeaders { get; set; }
}