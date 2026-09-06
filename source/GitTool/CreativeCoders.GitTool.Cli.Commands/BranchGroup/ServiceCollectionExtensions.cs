using CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup;

/// <summary>
/// Provides extension methods for registering the branch command services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required by the branch commands to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void AddGitBranchCommands(this IServiceCollection services)
    {
        services.TryAddTransient<IOrphanedLocalBranchesProvider, OrphanedLocalBranchesProvider>();
    }
}
