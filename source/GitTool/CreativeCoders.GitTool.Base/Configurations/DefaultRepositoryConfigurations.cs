using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CreativeCoders.Core.IO;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.GitTool.Base.Configurations;

internal class DefaultRepositoryConfigurations : IRepositoryConfigurations
{
    public RepositoryConfiguration GetConfiguration(IGitRepository gitRepository)
    {
        var configuration = LoadConfiguration(gitRepository.Info.RemoteUri);

        if (configuration != null)
        {
            return configuration;
        }

        var remote = gitRepository.Remotes[GitRemotes.Origin]
                     ?? gitRepository.Remotes.FirstOrDefault();

        var developRemoteName =
            (remote?.RefSpecs.FirstOrDefault()?.Destination ?? "refs/remotes/origin/*")
            .Replace("*", "develop");

        if (gitRepository.Branches[developRemoteName] == null)
        {
            return new RepositoryConfiguration
            {
                HasDevelopBranch = false
            };
        }

        return RepositoryConfiguration.Default;
    }

    private static RepositoryConfiguration? LoadConfiguration(Uri repositoryUrl)
    {
        var fileName = GetFileName(repositoryUrl);

        return FileSys.File.Exists(fileName)
            ? JsonSerializer.Deserialize<RepositoryConfiguration>(FileSys.File.ReadAllText(fileName))
            : null;
    }

    public async Task SaveConfigurationAsync(Uri repositoryUrl, RepositoryConfiguration configuration)
    {
        var fileName = GetFileName(repositoryUrl);
        
        FileSys.Directory.CreateDirectory(FileSys.Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException());

        await FileSys.File.WriteAllTextAsync(fileName,
            JsonSerializer.Serialize(configuration));
    }

    private static string GetFileName(Uri repositoryUrl)
    {
        return FileSys.Path.Combine(
            Env.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            GitToolApp.ConfigFolderName,
            RepositoryUrlToFileName(repositoryUrl));
    }

    private static string RepositoryUrlToFileName(Uri repositoryUrl)
    {
        var fileName = $"{GitToolApp.RepositoryConfigFilePrefix}{repositoryUrl}";

        foreach (var invalidChar in FileSys.Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar.ToString(), "-");
        }

        return $"{fileName}.json";
    }
}