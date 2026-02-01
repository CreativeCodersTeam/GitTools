using CreativeCoders.Cli.Hosting;
using CreativeCoders.Cli.Hosting.Help;
using CreativeCoders.Core.IO;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Cli.Commands.FeatureGroup;
using CreativeCoders.GitTool.Cli.Commands.Shared;
using CreativeCoders.GitTool.Cli.Commands.Tool.ShowConfig;
using CreativeCoders.GitTool.GitHub;
using CreativeCoders.GitTool.GitLab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Cli.App;

internal static class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var result = await CliHostBuilder.Create()
            .ConfigureServices(ConfigureServices)
            .EnableHelp(HelpCommandKind.CommandOrArgument)
            .ScanAssemblies(typeof(ShowConfigCommand).Assembly)
            .Build()
            .RunAsync(args).ConfigureAwait(false);

        return result.ExitCode;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = AddConfiguration(services);

        services.AddOptions();

        services.Configure<ToolConfiguration>(configuration.GetSection("tool"));

        ConfigureGitRepository(services);

        services.AddGitFeatureCommands();

        services.AddGitSharedCommands();

        services.AddGitTools();
        services.AddGitHubTools(configuration);
        services.AddGitLabTools(configuration);

        services.AddSingleton<ICml, Cml>();

        services.AddSysConsole();
    }

    private static void ConfigureGitRepository(IServiceCollection services)
    {
        services.TryAddSingleton(sp =>
        {
            var gitRepository = sp.GetRequiredService<IGitRepositoryFactory>().OpenRepositoryFromCurrentDir();

            var repositoryConfigurations = sp.GetRequiredService<IRepositoryConfigurations>();

            var repoConfiguration = repositoryConfigurations.GetConfiguration(gitRepository);

            if (repoConfiguration.DisableCertificateValidation)
            {
                gitRepository.CertificateCheck = (_, args) =>
                {
                    args.IsValid = true;
                    return true;
                };
            }

            return gitRepository;
        });
    }


    private static IConfiguration AddConfiguration(IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder();

        var toolConfigurationFile = FileSys.Path.Combine(
            Env.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            GitToolApp.ConfigFolderName,
            GitToolApp.ConfigFileName);

        if (FileSys.File.Exists(toolConfigurationFile))
        {
            configurationBuilder.AddJsonFile(toolConfigurationFile);
        }

        var configuration = configurationBuilder.Build();

        services.AddSingleton(configuration);
        services.AddSingleton<IConfiguration>(configuration);

        return configuration;
    }
}
