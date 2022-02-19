using System.Collections.Generic;
using System.Threading.Tasks;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Base
{
    public interface IGitServiceProviders
    {
        Task<IGitServiceProvider> GetServiceProviderAsync(IGitRepository gitRepository, string? providerName);

        IEnumerable<string> ProviderNames { get; }
    }
}
