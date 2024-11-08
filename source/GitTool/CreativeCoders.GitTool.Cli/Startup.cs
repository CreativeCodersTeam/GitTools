﻿using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Branches;
using CreativeCoders.GitTool.Commands.Features;
using CreativeCoders.GitTool.Commands.Releases;
using CreativeCoders.GitTool.Commands.Shared;
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

        services.AddGitFeatureCommands();

        services.AddGitSharedCommands();

        services.AddGitTools();
        services.AddGitHubTools(configuration);
        services.AddGitLabTools(configuration);

        services.AddSingleton(_ => AnsiConsole.Create(new AnsiConsoleSettings()));

        services.AddSingleton<ICml, Cml>();
    }

    public void Configure(ICliActionRuntimeBuilder runtimeBuilder)
    {
        runtimeBuilder.AddController<FeaturesController>();
        runtimeBuilder.AddController<BranchesController>();
        runtimeBuilder.AddController<ReleasesController>();
        runtimeBuilder.AddController<ToolController>();

        runtimeBuilder.UseMiddleware<GitToolsExceptionMiddleware>();

        runtimeBuilder.UseHelp();

        runtimeBuilder.UseRouting();
    }
}
