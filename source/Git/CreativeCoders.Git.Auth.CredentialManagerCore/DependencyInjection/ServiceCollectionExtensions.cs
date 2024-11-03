using CreativeCoders.Git.Abstractions.Auth;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.Git.Auth.CredentialManagerCore.DependencyInjection;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGcmCoreCredentialProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<IGcmCoreCredentialStore, DefaultGcmCoreCredentialStore>();
        services.TryAddSingleton<IGitCredentialProvider, DefaultGcmCoreCredentialProvider>();

        return services;
    }
}
