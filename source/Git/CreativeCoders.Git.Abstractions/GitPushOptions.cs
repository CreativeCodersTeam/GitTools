using System.Diagnostics.CodeAnalysis;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Represents options for a Git push operation.
/// </summary>
[ExcludeFromCodeCoverage]
public class GitPushOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether the remote branch should be created if it does not exist.
    /// </summary>
    /// <value><see langword="true"/> to create the remote branch if it does not exist; otherwise, <see langword="false"/>. The default is <see langword="true"/>.</value>
    public bool CreateRemoteBranchIfNotExists { get; set; } = true;
}