using System;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

/// <summary>
/// Default implementation of <see cref="IGitCredentialProvider"/> that retrieves Git credentials
/// from the Git Credential Manager Core credential store.
/// </summary>
internal class DefaultGcmCoreCredentialProvider : IGitCredentialProvider
{
    private readonly IGcmCoreCredentialStore _credentialStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultGcmCoreCredentialProvider"/> class.
    /// </summary>
    /// <param name="credentialStore">The credential store used to look up stored Git credentials.</param>
    public DefaultGcmCoreCredentialProvider(IGcmCoreCredentialStore credentialStore)
    {
        _credentialStore = Ensure.Argument(credentialStore).NotNull().Value;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"><paramref name="url"/> is <see langword="null"/> or empty.</exception>
    public IGitCredential? GetCredentials(string url, string? fromUrl)
    {
        Ensure.Argument(url).NotNullOrEmpty();

        var store = _credentialStore.Create("git");

        var credential = store.Get(ExtractServiceName(url), null);

        return credential == null
            ? null
            : new GitCredential(credential.Account, credential.Password);
    }

    /// <inheritdoc/>
    public string Name => "GcmCore";

    /// <summary>
    /// Extracts the scheme and host portion of a URL to use as a service name for credential lookup.
    /// </summary>
    /// <param name="url">The full URL from which to extract the service name.</param>
    /// <returns>A string in the form <c>scheme://host</c>.</returns>
    private static string ExtractServiceName(string url)
    {
        var uri = new Uri(url);

        return $"{uri.Scheme}://{uri.Host}";
    }
}
