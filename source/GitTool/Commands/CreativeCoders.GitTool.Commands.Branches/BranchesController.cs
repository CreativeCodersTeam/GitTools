using System.Threading.Tasks;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Update;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches;

[CliController("branch")]
public class BranchesController
{
    private readonly IListBranchesCommand _listBranchesCommand;

    private readonly IUpdateBranchesCommand _updateBranchesCommand;

    public BranchesController(IListBranchesCommand listBranchesCommand,
        IUpdateBranchesCommand updateBranchesCommand)
    {
        _listBranchesCommand = listBranchesCommand;
        _updateBranchesCommand = updateBranchesCommand;
    }

    [UsedImplicitly]
    [CliAction]
    [CliAction("list", HelpText = "List repository branches")]
    public async Task<CliActionResult> ListAsync(ListBranchesOptions options)
        => new(await _listBranchesCommand.ExecuteAsync(options));

    [UsedImplicitly]
    [CliAction("update", HelpText = "Update all permanent local branches by pulling from remote branches")]
    public async Task<CliActionResult> UpdateAsync(UpdateBranchesOptions options)
        => new(await _updateBranchesCommand.ExecuteAsync(options));
}