using CreativeCoders.GitTool.Commands.Branches.Commands.Info;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Pull;
using CreativeCoders.GitTool.Commands.Branches.Commands.Push;
using CreativeCoders.GitTool.Commands.Branches.Commands.Update;
using CreativeCoders.GitTool.Commands.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Commands.Branches;

public static class ServiceCollectionExtensions
{
    public static void AddGitBranchesCommand(this IServiceCollection services)
    {
        services.TryAddTransient<IInfoBranchesCommand, InfoBranchesCommand>();

        services.TryAddTransient<IListBranchesCommand, ListBranchesCommand>();

        services.TryAddTransient<IPullBranchCommand, PullBranchCommand>();

        services.TryAddTransient<IUpdateBranchesCommand, UpdateBranchesCommand>();

        services.TryAddTransient<IPushBranchCommand, PushBranchCommand>();

        services.AddGitSharedCommands();
    }
}
