using System.Diagnostics.CodeAnalysis;
using Microsoft.Git.CredentialManager;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

[ExcludeFromCodeCoverage]
internal class DefaultGcmCoreCredentialStore : IGcmCoreCredentialStore
{
    public ICredentialStore Create(string? credentialsNameSpace = default)
    {
        return CredentialStore.Create(credentialsNameSpace);
    }
}