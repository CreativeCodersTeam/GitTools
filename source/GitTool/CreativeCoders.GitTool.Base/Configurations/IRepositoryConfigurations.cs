using System;
using System.Threading.Tasks;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Base.Configurations
{
    public interface IRepositoryConfigurations
    {
        RepositoryConfiguration GetConfiguration(IGitRepository gitRepository);

        Task SaveConfigurationAsync(Uri repositoryUrl, RepositoryConfiguration configuration);
    }
}
