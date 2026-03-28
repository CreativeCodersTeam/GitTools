using CreativeCoders.Git;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Auth;
using CreativeCoders.Git.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Git services in the dependency injection container.
/// </summary>
public static class GitServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core Git services in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void AddGit(this IServiceCollection services)
    {
        services.TryAddTransient<ILibGitCaller, LibGitCaller>();

        services.TryAddSingleton<IGitRepositoryFactory, DefaultGitRepositoryFactory>();

        services.TryAddSingleton<IGitCredentialProviders, DefaultGitCredentialProviders>();

        services.TryAddSingleton<IGitRepositoryUtils, GitRepositoryUtils>();
    }

    /// <summary>
    /// Registers the core Git services and configures a transient <see cref="IGitRepository"/> using the specified options.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="setupOptions">An action to configure the <see cref="GitRepositoryOptions"/>.</param>
    /// <returns>The service collection for further chaining.</returns>
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
