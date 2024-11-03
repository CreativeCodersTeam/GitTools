using System;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit;

namespace CreativeCoders.GitTool.GitHub;

internal class DefaultGitHubServiceProviderFactory : IGitServiceProviderFactory
{
    private readonly GitHubServiceProviderOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public DefaultGitHubServiceProviderFactory(IServiceProvider serviceProvider,
        IOptions<GitHubServiceProviderOptions> options)
    {
        _serviceProvider = Ensure.NotNull(serviceProvider);
        _options = Ensure.NotNull(options).Value;
    }

    public Task<IGitServiceProvider> CreateProviderAsync(IGitRepository gitRepository)
    {
        return Task.FromResult<IGitServiceProvider>(
            new DefaultGitHubServiceProvider(_serviceProvider.GetRequiredService<IGitHubClient>()));
    }

    public bool IsResponsibleFor(IGitRepository gitRepository)
    {
        var repositoryHost = gitRepository.Info.RemoteUri.Host;

        return _options.Hosts.Concat(["github.com"])
            .Any(x => repositoryHost.Equals(x, StringComparison.CurrentCultureIgnoreCase));
    }

    public string ProviderName => ProviderNames.GitHub;
}
