using CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup;

public static class ServiceCollectionExtensions
{
    public static void AddGitFeatureCommands(this IServiceCollection services)
    {
        services.TryAddTransient<IFinishFeatureSteps, FinishFeatureSteps>();
    }
}
