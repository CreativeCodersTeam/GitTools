using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CreativeCoders.Git;

/// <summary>
/// Provides the default implementation of <see cref="IGitRepositoryFactory"/> for opening Git repositories.
/// </summary>
/// <param name="credentialProviders">The credential providers for authentication.</param>
/// <param name="repositoryUtils">The repository utilities for path discovery.</param>
/// <param name="serviceProvider">The service provider for resolving dependencies.</param>
internal class DefaultGitRepositoryFactory(
    IGitCredentialProviders credentialProviders,
    IGitRepositoryUtils repositoryUtils,
    IServiceProvider serviceProvider)
    : IGitRepositoryFactory
{
    private readonly IGitCredentialProviders _credentialProviders = Ensure.NotNull(credentialProviders);

    private readonly IGitRepositoryUtils _repositoryUtils = Ensure.NotNull(repositoryUtils);

    private readonly IServiceProvider _serviceProvider = Ensure.NotNull(serviceProvider);

    /// <inheritdoc />
    public IGitRepository OpenRepository(string? path)
    {
        var repo = path == null
            ? new Repository()
            : new Repository(_repositoryUtils.DiscoverGitPath(path)
                             ?? throw new GitNoRepositoryPathException(path));

        return new DefaultGitRepository(repo, _credentialProviders,
            _serviceProvider.GetRequiredService<ILibGitCaller>());
    }

    /// <inheritdoc />
    public IGitRepository OpenRepositoryFromCurrentDir()
    {
        return OpenRepository(Env.CurrentDirectory);
    }
}
