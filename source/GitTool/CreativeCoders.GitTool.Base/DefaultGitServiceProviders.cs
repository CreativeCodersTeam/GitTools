using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Exceptions;
using Microsoft.Extensions.Options;

namespace CreativeCoders.GitTool.Base;

internal class DefaultGitServiceProviders : IGitServiceProviders
{
    private readonly IEnumerable<IGitServiceProviderFactory> _providerFactories;

    public DefaultGitServiceProviders(IEnumerable<IGitServiceProviderFactory> providerFactories,
        IOptions<ToolConfiguration> toolOptions)
    {
        _providerFactories = Ensure.NotNull(providerFactories, nameof(providerFactories));
    }

    public async Task<IGitServiceProvider> GetServiceProviderAsync(IGitRepository gitRepository,
        string? providerName)
    {
        var providerFactory = GetProviderFactory(gitRepository, providerName);

        if (providerFactory == null)
        {
            throw new GitServiceProviderNotFoundException();
        }

        var gitServiceProvider = await providerFactory.CreateProviderAsync(gitRepository).ConfigureAwait(false);

        if (gitServiceProvider == null)
        {
            throw new GitServiceProviderNotFoundException();
        }

        return gitServiceProvider;
    }

    private IGitServiceProviderFactory? GetProviderFactory(IGitRepository gitRepository,
        string? providerName)
    {
        if (!string.IsNullOrEmpty(providerName))
        {
            return _providerFactories
                .FirstOrDefault(x =>
                    x.ProviderName.Equals(providerName, StringComparison.CurrentCultureIgnoreCase));
        }

        return _providerFactories.FirstOrDefault(x => x.IsResponsibleFor(gitRepository));
    }

    public IEnumerable<string> ProviderNames => _providerFactories.Select(x => x.ProviderName);
}