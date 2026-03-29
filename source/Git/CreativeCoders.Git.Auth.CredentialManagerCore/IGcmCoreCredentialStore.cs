using GitCredentialManager;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

/// <summary>
/// Provides a factory for creating <see cref="ICredentialStore"/> instances backed by the Git Credential Manager Core.
/// </summary>
public interface IGcmCoreCredentialStore
{
    /// <summary>
    /// Creates a new <see cref="ICredentialStore"/> instance for the specified credential namespace.
    /// </summary>
    /// <param name="credentialsNameSpace">The namespace used to scope credentials within the credential store.
    /// Pass <see langword="null"/> to use the default namespace.</param>
    /// <returns>An <see cref="ICredentialStore"/> scoped to the given <paramref name="credentialsNameSpace"/>.</returns>
    ICredentialStore Create(string? credentialsNameSpace = default);
}