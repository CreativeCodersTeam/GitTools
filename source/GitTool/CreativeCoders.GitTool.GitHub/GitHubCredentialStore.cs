using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Auth;
using Octokit;

namespace CreativeCoders.GitTool.GitHub;

internal class GitHubCredentialStore : ICredentialStore
{
    private readonly IGitCredentialProviders _credentialProviders;

    public GitHubCredentialStore(IGitCredentialProviders credentialProviders)
    {
        _credentialProviders = Ensure.Argument(credentialProviders, nameof(credentialProviders))
            .NotNull().Value;
    }

    public Task<Credentials?> GetCredentials()
    {
        var credentials = _credentialProviders.GetCredentials("https://github.com", null);

        var gitHubApiCredentials = credentials == null
            ? null
            : new Credentials(credentials.UserName, credentials.Password, AuthenticationType.Basic);

        return Task.FromResult(gitHubApiCredentials);
    }
}