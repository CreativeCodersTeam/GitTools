using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Releases.Commands.Create;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Releases;

[PublicAPI]
[CliController("release")]
public class ReleasesController
{
    private readonly ICreateReleaseCommand _createReleaseCommand;

    public ReleasesController(ICreateReleaseCommand createReleaseCommand)
    {
        _createReleaseCommand = Ensure.NotNull(createReleaseCommand, nameof(createReleaseCommand));
    }

    [CliAction("create")]
    public async Task<CliActionResult> CreateAsync(CreateReleaseOptions options)
    {
        var result = await _createReleaseCommand.ExecuteAsync(options);

        return new CliActionResult(result);
    }
}