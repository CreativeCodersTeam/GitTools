using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Releases.Commands.Create;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Releases;

[PublicAPI]
[CliController("release")]
public class ReleasesController
{
    private readonly IGitToolCommandExecutor _commandExecutor;

    public ReleasesController(IGitToolCommandExecutor commandExecutor)
    {
        _commandExecutor = Ensure.NotNull(commandExecutor, nameof(commandExecutor));
    }

    [CliAction("create")]
    public async Task<CliActionResult> CreateAsync(CreateReleaseOptions options)
        => new(await _commandExecutor.ExecuteAsync<CreateReleaseCommand, CreateReleaseOptions>(options));
}