namespace CreativeCoders.Git;

internal class GitRepositoryUtils : IGitRepositoryUtils
{
    public bool IsValidGitPath(string path) => Repository.IsValid(path);

    public string? DiscoverGitPath(string path) => Repository.Discover(path);

    public string? InitRepository(string path) => Repository.Init(path);

    public string? InitRepository(string path, bool isBare) => Repository.Init(path, isBare);
}