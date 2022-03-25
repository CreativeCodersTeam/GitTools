using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Branches;
using CreativeCoders.GitTool.Commands.Branches.Commands.Info;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Update;
using CreativeCoders.GitTool.Commands.Features;
using CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;
using CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;
using CreativeCoders.GitTool.Commands.Releases;
using CreativeCoders.GitTool.Commands.Releases.Commands.Create;
using CreativeCoders.GitTool.Commands.Tool;
using CreativeCoders.GitTool.GitHub;
using CreativeCoders.GitTool.GitLab;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Runtime;
using CreativeCoders.SysConsole.Cli.Actions.Runtime.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli;

public class Startup : ICliStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<ToolConfiguration>(configuration.GetSection("tool"));

        services.AddTransient<IStartFeatureCommand, StartFeatureCommand>();
        services.AddTransient<IFinishFeatureCommand, FinishFeatureCommand>();
        services.AddTransient<IFinishFeatureSteps, FinishFeatureSteps>();

        services.AddTransient<IListBranchesCommand, ListBranchesCommand>();

        services.AddTransient<ICreateReleaseCommand, CreateReleaseCommand>();

        services.AddTransient<IUpdateBranchesCommand, UpdateBranchesCommand>();

        services.AddTransient<IInfoBranchesCommand, InfoBranchesCommand>();

        services.AddGitTools();
        services.AddGitHubTools(configuration);
        services.AddGitLabTools(configuration);

        services.AddSingleton(_ => new AnsiConsoleFactory().Create(new AnsiConsoleSettings()));

        services.AddSingleton<ICml, Cml>();
    }

    public void Configure(ICliActionRuntimeBuilder runtimeBuilder)
    {
        runtimeBuilder.AddController<FeaturesController>();
        runtimeBuilder.AddController<BranchesController>();
        runtimeBuilder.AddController<ReleasesController>();
        runtimeBuilder.AddController<ToolController>();

        runtimeBuilder.UseMiddleware<GitToolsExceptionMiddleware>();

        runtimeBuilder.UseRouting();
    }
}