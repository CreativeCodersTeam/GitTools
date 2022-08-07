using CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;
using CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreativeCoders.GitTool.Commands.Features;

public static class ServiceCollectionExtensions
{
    public static void AddGitFeatureCommands(this IServiceCollection services)
    {
        services.TryAddTransient<IFinishFeatureSteps, FinishFeatureSteps>();
    }
}
