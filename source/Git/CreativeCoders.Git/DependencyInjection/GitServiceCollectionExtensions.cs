using CreativeCoders.Git;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Auth;
using CreativeCoders.Git.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class GitServiceCollectionExtensions
{
    public static void AddGit(this IServiceCollection services)
    {
        services.TryAddTransient<ILibGitCaller, LibGitCaller>();

        services.TryAddSingleton<IGitRepositoryFactory, DefaultGitRepositoryFactory>();

        services.TryAddSingleton<IGitCredentialProviders, DefaultGitCredentialProviders>();

        services.TryAddSingleton<IGitRepositoryUtils, GitRepositoryUtils>();
    }

    [PublicAPI]
    public static IServiceCollection AddGit(this IServiceCollection services,
        Action<GitRepositoryOptions> setupOptions)
    {
        Ensure.Argument(setupOptions).NotNull();

        services.AddGit();

        services.TryAddTransient(sp =>
        {
            var options = new GitRepositoryOptions();

            setupOptions(options);

            return sp.GetRequiredService<IGitRepositoryFactory>().OpenRepository(options.Path);
        });

        return services;
    }
}
