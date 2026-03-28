namespace CreativeCoders.Git;

/// <summary>
/// Provides the default implementation of <see cref="IGitRepositoryUtils"/> for Git path operations.
/// </summary>
internal class GitRepositoryUtils : IGitRepositoryUtils
{
    /// <inheritdoc />
    public bool IsValidGitPath(string path) => Repository.IsValid(path);

    /// <inheritdoc />
    public string? DiscoverGitPath(string path) => Repository.Discover(path);

    /// <inheritdoc />
    public string? InitRepository(string path) => Repository.Init(path);

    /// <inheritdoc />
    public string? InitRepository(string path, bool isBare) => Repository.Init(path, isBare);
}