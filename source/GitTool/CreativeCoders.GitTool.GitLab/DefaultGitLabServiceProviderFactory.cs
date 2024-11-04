using System;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.GitTool.Base;
using GitLabApiClient;
using Microsoft.Extensions.Options;

namespace CreativeCoders.GitTool.GitLab;

internal class DefaultGitLabServiceProviderFactory : IGitServiceProviderFactory
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly GitLabServiceProviderOptions _options;

    public DefaultGitLabServiceProviderFactory(IGitCredentialProviders credentialProviders,
        IOptions<GitLabServiceProviderOptions> options)
    {
        _credentialProviders = Ensure.NotNull(credentialProviders);
        _options = Ensure.NotNull(options).Value;
    }

    public async Task<IGitServiceProvider> CreateProviderAsync(IGitRepository gitRepository)
    {
        Ensure.NotNull(gitRepository);

        var remote = gitRepository.Remotes[GitRemotes.Origin] ?? gitRepository.Remotes.FirstOrDefault();

        if (remote == null)
        {
            throw new GitNoRemoteFoundException();
        }

        var remoteUri = new Uri(remote.Url);

        var credential = _credentialProviders.GetCredentials(remote.Url, null);

        var client = new GitLabClient($"{remoteUri.Scheme}://{remoteUri.Host}");

        if (credential != null)
        {
            await client.LoginAsync(credential.UserName, credential.Password);
        }

        return new DefaultGitLabServiceProvider(client);
    }

    public bool IsResponsibleFor(IGitRepository gitRepository)
    {
        var repositoryHost = gitRepository.Info.RemoteUri.Host;

        return _options.Hosts.Concat(["gitlab.com"])
            .Any(x => repositoryHost.Equals(x, StringComparison.CurrentCultureIgnoreCase));
    }

    public string ProviderName => ProviderNames.GitLab;
}
