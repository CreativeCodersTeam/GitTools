using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Branches.Commands.Info;
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

    private readonly IInfoBranchesCommand _infoBranchesCommand;

    public BranchesController(IListBranchesCommand listBranchesCommand,
        IUpdateBranchesCommand updateBranchesCommand, IInfoBranchesCommand infoBranchesCommand)
    {
        _listBranchesCommand = Ensure.NotNull(listBranchesCommand, nameof(listBranchesCommand));
        _updateBranchesCommand = Ensure.NotNull(updateBranchesCommand, nameof(updateBranchesCommand));
        _infoBranchesCommand = Ensure.NotNull(infoBranchesCommand, nameof(infoBranchesCommand));
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

    [UsedImplicitly]
    [CliAction("info", HelpText = "Show infos of current branch")]
    public async Task<CliActionResult> InfoAsync()
        => new(await _infoBranchesCommand.ExecuteAsync());
}