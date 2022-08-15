using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;
using CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Features;

[CliController("feature")]
public class FeaturesController
{
    private readonly IGitToolCommandExecutor _commandExecutor;

    public FeaturesController(IGitToolCommandExecutor commandExecutor)
    {
        _commandExecutor = Ensure.NotNull(commandExecutor, nameof(commandExecutor));
    }

    [UsedImplicitly]
    [CliAction("start", HelpText = "Start a new feature")]
    public async Task<CliActionResult> StartAsync(StartFeatureOptions options)
        => new(await _commandExecutor.ExecuteAsync<StartFeatureCommand, StartFeatureOptions>(options));

    [UsedImplicitly]
    [CliAction("finish", HelpText = "Finish an active feature")]
    public async Task<CliActionResult> FinishAsync(FinishFeatureOptions options)
        => new(await _commandExecutor.ExecuteAsync<FinishFeatureCommand, FinishFeatureOptions>(options));
}