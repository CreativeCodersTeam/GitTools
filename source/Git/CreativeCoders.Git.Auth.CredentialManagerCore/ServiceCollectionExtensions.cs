using CreativeCoders.Git.Abstractions.Auth;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

/// <summary>
/// Provides extension methods for registering Git Credential Manager Core services
/// with an <see cref="IServiceCollection"/>.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Git Credential Manager Core credential provider and its dependencies
    /// as singletons in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, allowing further chaining of registrations.</returns>
    /// <remarks>
    /// Registers <see cref="IGcmCoreCredentialStore"/> with <c>DefaultGcmCoreCredentialStore</c>
    /// and <see cref="IGitCredentialProvider"/> with <c>DefaultGcmCoreCredentialProvider</c>.
    /// Existing registrations are not overwritten.
    /// </remarks>
    public static IServiceCollection AddGcmCoreCredentialProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<IGcmCoreCredentialStore, DefaultGcmCoreCredentialStore>();
        services.TryAddSingleton<IGitCredentialProvider, DefaultGcmCoreCredentialProvider>();

        return services;
    }
}
