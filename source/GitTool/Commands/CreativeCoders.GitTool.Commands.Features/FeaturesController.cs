using System.Threading.Tasks;
using CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;
using CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Features;

[CliController("feature")]
public class FeaturesController
{
    private readonly IStartFeatureCommand _startFeatureCommand;

    private readonly IFinishFeatureCommand _finishFeatureCommand;

    public FeaturesController(IStartFeatureCommand startFeatureCommand,
        IFinishFeatureCommand finishFeatureCommand)
    {
        _startFeatureCommand = startFeatureCommand;
        _finishFeatureCommand = finishFeatureCommand;
    }

    [UsedImplicitly]
    [CliAction("start", HelpText = "Start a new feature")]
    public async Task<CliActionResult> StartAsync(StartFeatureOptions options)
    {
        var result = await _startFeatureCommand.StartFeatureAsync(options);

        return new CliActionResult(result);
    }

    [UsedImplicitly]
    [CliAction("finish", HelpText = "Finish an active feature")]
    public async Task<CliActionResult> FinishAsync(FinishFeatureOptions options)
    {
        var result = await _finishFeatureCommand.FinishFeatureAsync(options);

        return new CliActionResult(result);
    }
}