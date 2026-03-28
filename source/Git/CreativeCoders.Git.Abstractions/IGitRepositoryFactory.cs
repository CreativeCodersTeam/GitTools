using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Provides factory methods for creating <see cref="IGitRepository"/> instances.
/// </summary>
[PublicAPI]
public interface IGitRepositoryFactory
{
    /// <summary>
    /// Opens a Git repository at the specified path.
    /// </summary>
    /// <param name="path">The path to the repository, or <see langword="null"/> to use the current directory.</param>
    /// <returns>The opened repository.</returns>
    IGitRepository OpenRepository(string? path);

    /// <summary>
    /// Opens a Git repository from the current working directory.
    /// </summary>
    /// <returns>The opened repository.</returns>
    IGitRepository OpenRepositoryFromCurrentDir();
}
