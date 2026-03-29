using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

/// <summary>
/// Provides the default implementation of <see cref="IGitCredentialProviders"/> that aggregates multiple credential providers.
/// </summary>
/// <param name="providers">The collection of registered credential providers.</param>
internal class DefaultGitCredentialProviders(IEnumerable<IGitCredentialProvider> providers) : IGitCredentialProviders
{
    private readonly IEnumerable<IGitCredentialProvider> _providers = Ensure.NotNull(providers);

    /// <inheritdoc />
    public IGitCredentialProvider? GetProvider(string providerName)
    {
        return _providers.FirstOrDefault(x =>
            x.Name.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <inheritdoc />
    public IGitCredential? GetCredentials(string url, string? fromUrl)
    {
        return _providers
            .Select(gitCredentialProvider =>
                gitCredentialProvider.GetCredentials(url, fromUrl))
            .FirstOrDefault(credential => credential != null);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage] public string Name => "Default";
}
