using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Exceptions;

namespace CreativeCoders.Git;

internal class DefaultGitRepositoryFactory : IGitRepositoryFactory
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly IGitRepositoryUtils _repositoryUtils;

    public DefaultGitRepositoryFactory(IGitCredentialProviders credentialProviders,
        IGitRepositoryUtils repositoryUtils)
    {
        _credentialProviders = Ensure.NotNull(credentialProviders, nameof(credentialProviders));
        _repositoryUtils = Ensure.NotNull(repositoryUtils, nameof(repositoryUtils));
    }

    public IGitRepository OpenRepository(string? path)
    {
        var repo = path == null
            ? new Repository()
            : new Repository(_repositoryUtils.DiscoverGitPath(path)
                             ?? throw new GitNoRepositoryPathException(path));

        return new DefaultGitRepository(repo, _credentialProviders);
    }

    public IGitRepository OpenRepositoryFromCurrentDir()
    {
        return OpenRepository(Env.CurrentDirectory);
    }
}