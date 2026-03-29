using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Auth;

/// <summary>
/// Represents a collection of <see cref="IGitCredentialProvider"/> instances and provides lookup by name.
/// </summary>
[PublicAPI]
public interface IGitCredentialProviders : IGitCredentialProvider
{
    /// <summary>
    /// Gets the credential provider with the specified name.
    /// </summary>
    /// <param name="providerName">The name of the provider to retrieve.</param>
    /// <returns>The matching credential provider, or <see langword="null"/> if no provider with the specified name exists.</returns>
    IGitCredentialProvider? GetProvider(string providerName);
}