using CreativeCoders.Core;
using CreativeCoders.GitTool.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Octokit;

namespace CreativeCoders.GitTool.GitHub;

public static class GitHubServiceCollectionExtensions
{
    public static void AddGitHubTools(this IServiceCollection services, IConfiguration configuration)
    {
        Ensure.NotNull(configuration);

        services.AddGitTools();

        services.AddTransient<IGitHubClient>(sp =>
        {
            var credentialStore = sp.GetRequiredService<ICredentialStore>();

            return new GitHubClient(new ProductHeaderValue("CreativeCoders.GitTool"), credentialStore);
        });

        services.TryAddSingleton<ICredentialStore, GitHubCredentialStore>();

        services.AddTransient<IGitServiceProviderFactory, DefaultGitHubServiceProviderFactory>();

        services.Configure<GitHubServiceProviderOptions>(
            configuration.GetSection("GitServiceProviders").GetSection("GitHub"));
    }
}
