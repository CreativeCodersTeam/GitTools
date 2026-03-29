using System.Security;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

/// <summary>
/// Handles credential requests from LibGit2Sharp by delegating to registered <see cref="IGitCredentialProviders"/>.
/// </summary>
/// <param name="credentialProviders">The credential providers to query for credentials.</param>
public class GitCredentialsHandler(IGitCredentialProviders credentialProviders)
{
    private readonly IGitCredentialProviders _credentialProviders = Ensure.NotNull(credentialProviders);

    /// <summary>
    /// Handles a credential request by resolving credentials from the registered providers.
    /// </summary>
    /// <param name="url">The URL of the remote repository.</param>
    /// <param name="usernameFromUrl">The username extracted from the URL, or <see langword="null"/>.</param>
    /// <param name="types">The supported credential types.</param>
    /// <returns>The resolved credentials, or <see langword="null"/> if no credentials are available.</returns>
    public Credentials? HandleCredentials(string url, string? usernameFromUrl, SupportedCredentialTypes types)
    {
        var credential = _credentialProviders.GetCredentials(url, usernameFromUrl);

        if (credential == null || types == SupportedCredentialTypes.Default)
        {
            return types.HasFlag(SupportedCredentialTypes.Default) ? new DefaultCredentials() : null;
        }

        return new SecureUsernamePasswordCredentials
        {
            Username = credential.UserName,
            Password = TextToSecure(credential.Password)
        };
    }

    private static SecureString TextToSecure(string password)
    {
        var secureString = new SecureString();

        foreach (var passwordChar in password)
        {
            secureString.AppendChar(passwordChar);
        }

        secureString.MakeReadOnly();

        return secureString;
    }
}
