using CreativeCoders.Git.Abstractions.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.Git.Auth.CredentialManagerCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGcmCoreCredentialProvider(this IServiceCollection services)
        {
            services.TryAddSingleton<IGcmCoreCredentialStore, DefaultGcmCoreCredentialStore>();
            services.TryAddSingleton<IGitCredentialProvider, DefaultGcmCoreCredentialProvider>();

            return services;
        }
    }
}
