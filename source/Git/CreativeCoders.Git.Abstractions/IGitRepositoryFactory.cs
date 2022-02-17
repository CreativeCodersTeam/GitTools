using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions
{
    [PublicAPI]
    public interface IGitRepositoryFactory
    {
        IGitRepository OpenRepository(string? path);

        IGitRepository OpenRepositoryFromCurrentDir();
    }
}
