using CreativeCoders.Cli.Hosting;
using CreativeCoders.Cli.Hosting.Help;
using CreativeCoders.Core.IO;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Cli.Commands.Tool;
using CreativeCoders.GitTool.GitHub;
using CreativeCoders.GitTool.GitLab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

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

        //services.AddGitFeatureCommands();

        //services.AddGitSharedCommands();

        services.AddGitTools();
        services.AddGitHubTools(configuration);
        services.AddGitLabTools(configuration);

        services.AddSingleton(_ => AnsiConsole.Create(new AnsiConsoleSettings()));

        services.AddSingleton<ICml, Cml>();
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
