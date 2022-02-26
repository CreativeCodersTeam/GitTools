using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

internal class DefaultGitCredentialProviders : IGitCredentialProviders
{
    private readonly IEnumerable<IGitCredentialProvider> _providers;

    public DefaultGitCredentialProviders(IEnumerable<IGitCredentialProvider> providers)
    {
        _providers = Ensure.Argument(providers, nameof(providers)).NotNull().Value;
    }

    public IGitCredentialProvider? GetProvider(string providerName)
    {
        return _providers.FirstOrDefault(x =>
            x.Name.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));
    }

    public IGitCredential? GetCredentials(string url, string? fromUrl)
    {
        return _providers
            .Select(gitCredentialProvider =>
                gitCredentialProvider.GetCredentials(url, fromUrl))
            .FirstOrDefault(credential => credential != null);
    }

    [ExcludeFromCodeCoverage] public string Name => "Default";
}