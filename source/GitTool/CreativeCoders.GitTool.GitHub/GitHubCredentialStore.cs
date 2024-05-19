using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Auth;
using Octokit;

namespace CreativeCoders.GitTool.GitHub;

internal class GitHubCredentialStore : ICredentialStore
{
    [SuppressMessage("", "S1075", Justification = "Github url will not change this often")]
    private const string GitHubUrl = "https://github.com";

    private readonly IGitCredentialProviders _credentialProviders;

    public GitHubCredentialStore(IGitCredentialProviders credentialProviders)
    {
        _credentialProviders = Ensure.Argument(credentialProviders)
            .NotNull().Value;
    }

    public Task<Credentials?> GetCredentials()
    {
        var credentials = _credentialProviders.GetCredentials(GitHubUrl, null);

        var gitHubApiCredentials = credentials == null
            ? null
            : new Credentials(credentials.UserName, credentials.Password, AuthenticationType.Basic);

        return Task.FromResult(gitHubApiCredentials);
    }
}
