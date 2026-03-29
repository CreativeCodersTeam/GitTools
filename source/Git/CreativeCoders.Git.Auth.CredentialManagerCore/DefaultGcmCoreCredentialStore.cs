using System.Diagnostics.CodeAnalysis;
using GitCredentialManager;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

/// <summary>
/// Default implementation of <see cref="IGcmCoreCredentialStore"/> that delegates to
/// the Git Credential Manager Core library.
/// </summary>
[ExcludeFromCodeCoverage]
internal class DefaultGcmCoreCredentialStore : IGcmCoreCredentialStore
{
    /// <inheritdoc/>
    public ICredentialStore Create(string? credentialsNameSpace = null)
    {
        return CredentialManager.Create(credentialsNameSpace);
    }
}
