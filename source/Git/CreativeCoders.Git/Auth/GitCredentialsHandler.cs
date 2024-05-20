using System.Security;
using CreativeCoders.Git.Abstractions.Auth;

namespace CreativeCoders.Git.Auth;

public class GitCredentialsHandler
{
    private readonly IGitCredentialProviders _credentialProviders;

    public GitCredentialsHandler(IGitCredentialProviders credentialProviders)
    {
        _credentialProviders = Ensure
            .Argument(credentialProviders)
            .NotNull()
            .Value;
    }

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
