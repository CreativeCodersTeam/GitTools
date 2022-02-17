using System.Threading.Tasks;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Base
{
    public interface IGitServiceProviderFactory
    {
        Task<IGitServiceProvider> CreateProviderAsync(IGitRepository gitRepository);

        bool IsResponsibleFor(IGitRepository gitRepository);

        string ProviderName { get; }
    }
}
