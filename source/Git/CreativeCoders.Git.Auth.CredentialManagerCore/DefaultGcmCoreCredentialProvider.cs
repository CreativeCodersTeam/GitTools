using System;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

internal class DefaultGcmCoreCredentialProvider : IGitCredentialProvider
{
    private readonly IGcmCoreCredentialStore _credentialStore;

    public DefaultGcmCoreCredentialProvider(IGcmCoreCredentialStore credentialStore)
    {
        _credentialStore = Ensure.Argument(credentialStore, nameof(credentialStore)).NotNull().Value;
    }

    public IGitCredential? GetCredentials(string url, string? fromUrl)
    {
        Ensure.Argument(url, nameof(url)).NotNullOrEmpty();

        var store = _credentialStore.Create("git");

        var credential = store.Get(ExtractServiceName(url), null);

        return credential == null
            ? null
            : new GitCredential(credential.Account, credential.Password);
    }

    private static string ExtractServiceName(string url)
    {
        var uri = new Uri(url);

        return $"{uri.Scheme}://{uri.Host}";
    }

    public string Name => "GcmCore";
}