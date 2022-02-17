using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth
{
    [ExcludeFromCodeCoverage]
    public class GitCredential : IGitCredential
    {
        public GitCredential(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }

        public string Password { get; }
    }
}
