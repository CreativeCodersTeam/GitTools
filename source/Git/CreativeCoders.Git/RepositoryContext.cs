using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

/// <summary>
/// Holds the shared context for repository operations, providing access to the underlying LibGit2Sharp repository,
/// credentials, signatures, and certificate handlers.
/// </summary>
/// <param name="repository">The parent <see cref="DefaultGitRepository"/> instance.</param>
/// <param name="libGitRepository">The underlying LibGit2Sharp repository.</param>
/// <param name="libGitCaller">The caller used to invoke LibGit2Sharp operations with exception handling.</param>
/// <param name="getSignature">A factory function that returns the current commit signature.</param>
/// <param name="getCredentialsHandler">A factory function that returns the credentials handler.</param>
/// <param name="certificateCheckHandler">A factory function that returns the certificate check handler.</param>
internal class RepositoryContext(
    DefaultGitRepository repository,
    Repository libGitRepository,
    ILibGitCaller libGitCaller,
    Func<Signature> getSignature,
    Func<CredentialsHandler> getCredentialsHandler,
    Func<CertificateCheckHandler?> certificateCheckHandler)
{
    private readonly Func<CertificateCheckHandler?> _certificateCheckHandler = Ensure.NotNull(certificateCheckHandler);

    private readonly Func<CredentialsHandler> _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler);

    private readonly Func<Signature> _getSignature = Ensure.NotNull(getSignature);

    /// <summary>
    /// Gets the current commit signature.
    /// </summary>
    /// <returns>The signature for the current user.</returns>
    public Signature GetSignature()
    {
        return _getSignature();
    }

    /// <summary>
    /// Gets the credentials handler for authentication.
    /// </summary>
    /// <returns>The credentials handler.</returns>
    public CredentialsHandler GetCredentialsHandler()
    {
        return _getCredentialsHandler();
    }

    /// <summary>
    /// Gets the certificate check handler for SSL/SSH verification.
    /// </summary>
    /// <returns>The certificate check handler, or <see langword="null"/> if no custom handler is configured.</returns>
    public CertificateCheckHandler? GetCertificateCheckHandler()
    {
        return _certificateCheckHandler();
    }

    /// <summary>
    /// Gets the underlying LibGit2Sharp repository.
    /// </summary>
    public Repository LibGitRepository { get; } = Ensure.NotNull(libGitRepository);

    /// <summary>
    /// Gets the caller used to invoke LibGit2Sharp operations with exception handling.
    /// </summary>
    public ILibGitCaller LibGitCaller { get; } = Ensure.NotNull(libGitCaller);

    /// <summary>
    /// Gets the parent <see cref="DefaultGitRepository"/> instance.
    /// </summary>
    public DefaultGitRepository Repository { get; } = Ensure.NotNull(repository);
}
