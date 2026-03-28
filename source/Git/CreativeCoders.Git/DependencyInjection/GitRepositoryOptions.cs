using System.Diagnostics.CodeAnalysis;

namespace CreativeCoders.Git.DependencyInjection;

/// <summary>
/// Represents the configuration options for opening a Git repository via dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public class GitRepositoryOptions
{
    /// <summary>
    /// Gets or sets the file system path to the Git repository.
    /// </summary>
    /// <value>The repository path. The default is <see cref="string.Empty"/>.</value>
    public string Path { get; set; } = string.Empty;
}