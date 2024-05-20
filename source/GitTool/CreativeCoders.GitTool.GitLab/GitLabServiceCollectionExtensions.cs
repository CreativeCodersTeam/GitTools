using CreativeCoders.Core;
using CreativeCoders.GitTool.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreativeCoders.GitTool.GitLab;

public static class GitLabServiceCollectionExtensions
{
    public static IServiceCollection AddGitLabTools(this IServiceCollection services,
        IConfiguration configuration)
    {
        Ensure.NotNull(configuration);

        services.AddGitTools();

        services.AddTransient<IGitServiceProviderFactory, DefaultGitLabServiceProviderFactory>();

        services.Configure<GitLabServiceProviderOptions>(
            configuration.GetSection("GitServiceProviders").GetSection("GitLab"));

        return services;
    }
}
