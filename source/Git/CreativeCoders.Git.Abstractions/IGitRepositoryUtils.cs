using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions
{
    [PublicAPI]
    public interface IGitRepositoryUtils
    {
        bool IsValidGitPath(string path);

        string? DiscoverGitPath(string path);

        string? InitRepository(string path);

        string? InitRepository(string path, bool isBare);
    }
}
