using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Branches.Commands.Info;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Pull;
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

    private readonly IPullBranchCommand _pullBranchCommand;

    public BranchesController(IListBranchesCommand listBranchesCommand,
        IUpdateBranchesCommand updateBranchesCommand, IInfoBranchesCommand infoBranchesCommand,
        IPullBranchCommand pullBranchCommand)
    {
        _listBranchesCommand = Ensure.NotNull(listBranchesCommand, nameof(listBranchesCommand));
        _updateBranchesCommand = Ensure.NotNull(updateBranchesCommand, nameof(updateBranchesCommand));
        _infoBranchesCommand = Ensure.NotNull(infoBranchesCommand, nameof(infoBranchesCommand));
        _pullBranchCommand = Ensure.NotNull(pullBranchCommand, nameof(pullBranchCommand));
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
    public async Task<CliActionResult> InfoAsync(InfoBranchesOptions options)
        => new(await _infoBranchesCommand.ExecuteAsync(options));

    [UsedImplicitly]
    [CliAction("pull", HelpText = "Pulls updates for current branch from remote")]
    public async Task<CliActionResult> PullAsync()
        => new(await _pullBranchCommand.ExecuteAsync());
}