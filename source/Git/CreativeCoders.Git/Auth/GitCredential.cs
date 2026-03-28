using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

/// <summary>
/// Represents a Git credential consisting of a username and password.
/// </summary>
/// <param name="userName">The username.</param>
/// <param name="password">The password.</param>
[ExcludeFromCodeCoverage]
public class GitCredential(string userName, string password) : IGitCredential
{
    /// <inheritdoc />
    public string UserName { get; } = userName;

    /// <inheritdoc />
    public string Password { get; } = password;
}
