using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

[ExcludeFromCodeCoverage]
public class GitCredential(string userName, string password) : IGitCredential
{
    public string UserName { get; } = userName;

    public string Password { get; } = password;
}
