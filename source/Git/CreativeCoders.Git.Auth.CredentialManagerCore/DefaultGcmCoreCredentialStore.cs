using System.Diagnostics.CodeAnalysis;
using GitCredentialManager;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

[ExcludeFromCodeCoverage]
internal class DefaultGcmCoreCredentialStore : IGcmCoreCredentialStore
{
    public ICredentialStore Create(string? credentialsNameSpace = default)
    {
        return CredentialManager.Create(credentialsNameSpace);
    }
}
