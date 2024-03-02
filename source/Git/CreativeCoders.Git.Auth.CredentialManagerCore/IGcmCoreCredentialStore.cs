using GitCredentialManager;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

public interface IGcmCoreCredentialStore
{
    ICredentialStore Create(string? credentialsNameSpace = default);
}