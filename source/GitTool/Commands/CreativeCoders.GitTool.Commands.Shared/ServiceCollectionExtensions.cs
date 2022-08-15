using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Commands.Shared;

public static class ServiceCollectionExtensions
{
    public static void AddGitSharedCommands(this IServiceCollection services)
    {
        services.TryAddTransient<IGitToolPullCommand, GitToolPullCommand>();

        services.TryAddTransient<IGitToolPushCommand, GitToolPushCommand>();

        services.TryAddTransient<IGitToolCommandExecutor, GitToolCommandExecutor>();
    }
}
