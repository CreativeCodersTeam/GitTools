namespace CreativeCoders.Git.Abstractions.Auth;

/// <summary>
/// Represents a Git credential consisting of a user name and password.
/// </summary>
public interface IGitCredential
{
    /// <summary>
    /// Gets the user name.
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Gets the password.
    /// </summary>
    string Password { get; }
}