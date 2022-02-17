using CreativeCoders.Git.Auth.CredentialManagerCore.DependencyInjection;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class GitToolsServiceCollectionExtensions
    {
        public static IServiceCollection AddGitTools(this IServiceCollection services)
        {
            services.TryAddSingleton<IRepositoryConfigurations, DefaultRepositoryConfigurations>();

            services.AddGit();

            services.AddGcmCoreCredentialProvider();

            services.TryAddTransient<IGitServiceProviders, DefaultGitServiceProviders>();

            return services;
        }
    }
}
