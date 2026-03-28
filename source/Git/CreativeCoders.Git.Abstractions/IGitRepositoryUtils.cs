using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Provides utility methods for discovering, validating, and initializing Git repositories.
/// </summary>
[PublicAPI]
public interface IGitRepositoryUtils
{
    /// <summary>
    /// Determines whether the specified path is a valid Git repository.
    /// </summary>
    /// <param name="path">The file system path to check.</param>
    /// <returns><see langword="true"/> if the path is a valid Git repository; otherwise, <see langword="false"/>.</returns>
    bool IsValidGitPath(string path);

    /// <summary>
    /// Discovers the Git repository path by searching upward from the specified path.
    /// </summary>
    /// <param name="path">The starting path to search from.</param>
    /// <returns>The discovered repository path, or <see langword="null"/> if no repository is found.</returns>
    string? DiscoverGitPath(string path);

    /// <summary>
    /// Initializes a new Git repository at the specified path.
    /// </summary>
    /// <param name="path">The file system path where the repository should be created.</param>
    /// <returns>The path to the initialized repository, or <see langword="null"/> if initialization failed.</returns>
    string? InitRepository(string path);

    /// <summary>
    /// Initializes a new Git repository at the specified path.
    /// </summary>
    /// <param name="path">The file system path where the repository should be created.</param>
    /// <param name="isBare"><see langword="true"/> to create a bare repository; otherwise, <see langword="false"/>.</param>
    /// <returns>The path to the initialized repository, or <see langword="null"/> if initialization failed.</returns>
    string? InitRepository(string path, bool isBare);
}