using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Base
{
    internal class DefaultGitServiceProviders : IGitServiceProviders
    {
        private readonly IEnumerable<IGitServiceProviderFactory> _providerFactories;

        public DefaultGitServiceProviders(IEnumerable<IGitServiceProviderFactory> providerFactories)
        {
            _providerFactories = Ensure.NotNull(providerFactories, nameof(providerFactories));
        }

        public async Task<IGitServiceProvider?> GetServiceProviderAsync(IGitRepository gitRepository,
            string? providerName)
        {
            if (!string.IsNullOrEmpty(providerName))
            {
                return await _providerFactories
                    .First(x => x.ProviderName.Equals(providerName, StringComparison.CurrentCultureIgnoreCase))
                    .CreateProviderAsync(gitRepository);
            }

            return await _providerFactories.First(x => x.IsResponsibleFor(gitRepository))
                .CreateProviderAsync(gitRepository);
        }

        public IEnumerable<string> ProviderNames => _providerFactories.Select(x => x.ProviderName);
    }
}
