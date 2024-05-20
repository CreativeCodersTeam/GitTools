using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CreativeCoders.Git;

internal class DefaultGitRepositoryFactory : IGitRepositoryFactory
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly IGitRepositoryUtils _repositoryUtils;

    private readonly IServiceProvider _serviceProvider;

    public DefaultGitRepositoryFactory(IGitCredentialProviders credentialProviders,
        IGitRepositoryUtils repositoryUtils, IServiceProvider serviceProvider)
    {
        _credentialProviders = Ensure.NotNull(credentialProviders);
        _repositoryUtils = Ensure.NotNull(repositoryUtils);
        _serviceProvider = Ensure.NotNull(serviceProvider);
    }

    public IGitRepository OpenRepository(string? path)
    {
        var repo = path == null
            ? new Repository()
            : new Repository(_repositoryUtils.DiscoverGitPath(path)
                             ?? throw new GitNoRepositoryPathException(path));

        return new DefaultGitRepository(repo, _credentialProviders,
            _serviceProvider.GetRequiredService<ILibGitCaller>());
    }

    public IGitRepository OpenRepositoryFromCurrentDir()
    {
        return OpenRepository(Env.CurrentDirectory);
    }
}
