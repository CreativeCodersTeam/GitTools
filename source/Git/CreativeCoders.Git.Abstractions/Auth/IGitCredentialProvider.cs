namespace CreativeCoders.Git.Abstractions.Auth;

/// <summary>
/// Provides credentials for authenticating against a Git remote.
/// </summary>
public interface IGitCredentialProvider
{
    /// <summary>
    /// Gets the credentials for the specified URL.
    /// </summary>
    /// <param name="url">The URL of the remote resource.</param>
    /// <param name="fromUrl">The URL that initiated the credential request, or <see langword="null"/> if not available.</param>
    /// <returns>The credential, or <see langword="null"/> if no credential is available.</returns>
    IGitCredential? GetCredentials(string url, string? fromUrl);

    /// <summary>
    /// Gets the name of the credential provider.
    /// </summary>
    string Name { get; }
}