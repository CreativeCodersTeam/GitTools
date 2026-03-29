namespace CreativeCoders.Git.Abstractions.GitCommands;

/// <summary>
/// Represents options for a fetch tags command.
/// </summary>
public class FetchTagsCommandOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether tags that no longer exist on the remote should be deleted locally.
    /// </summary>
    /// <value><see langword="true"/> to prune deleted remote tags; otherwise, <see langword="false"/>. The default is <see langword="true"/>.</value>
    public bool Prune { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the remote to fetch tags from.
    /// </summary>
    /// <value>The remote name. The default is <c>"origin"</c>.</value>
    public string RemoteName { get; set; } = "origin";
}
