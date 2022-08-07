﻿using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Branches.Commands.Info;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Pull;
using CreativeCoders.GitTool.Commands.Branches.Commands.Push;
using CreativeCoders.GitTool.Commands.Branches.Commands.Update;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches;

[CliController("branch")]
public class BranchesController
{
    private readonly IUpdateBranchesCommand _updateBranchesCommand;

    private readonly IPullBranchCommand _pullBranchCommand;

    private readonly IPushBranchCommand _pushBranchCommand;

    private readonly IGitToolCommandExecutor _commandExecutor;

    public BranchesController(IUpdateBranchesCommand updateBranchesCommand,
        IPullBranchCommand pullBranchCommand, IPushBranchCommand pushBranchCommand, IGitToolCommandExecutor commandExecutor)
    {
        _updateBranchesCommand = Ensure.NotNull(updateBranchesCommand, nameof(updateBranchesCommand));
        _pullBranchCommand = Ensure.NotNull(pullBranchCommand, nameof(pullBranchCommand));
        _pushBranchCommand = Ensure.NotNull(pushBranchCommand, nameof(pushBranchCommand));
        _commandExecutor = Ensure.NotNull(commandExecutor, nameof(commandExecutor));
    }

    [UsedImplicitly]
    [CliAction]
    [CliAction("list", HelpText = "List repository branches")]
    public async Task<CliActionResult> ListAsync(ListBranchesOptions options)
        => new(await _commandExecutor.ExecuteAsync<ListBranchesCommand, ListBranchesOptions>(options));

    [UsedImplicitly]
    [CliAction("update", HelpText = "Update all permanent local branches by pulling from remote branches")]
    public async Task<CliActionResult> UpdateAsync(UpdateBranchesOptions options)
        => new(await _updateBranchesCommand.ExecuteAsync(options));

    [UsedImplicitly]
    [CliAction("info", HelpText = "Show infos of current branch")]
    public async Task<CliActionResult> InfoAsync(InfoBranchesOptions options)
        => new(await _commandExecutor.ExecuteAsync<InfoBranchesCommand, InfoBranchesOptions>(options));

    [UsedImplicitly]
    [CliAction("pull", HelpText = "Pulls updates for current branch from remote")]
    public async Task<CliActionResult> PullAsync(PullBranchOptions options)
        => new(await _pullBranchCommand.ExecuteAsync(options));

    [UsedImplicitly]
    [CliAction("push", HelpText = "Pushes updates from current branch to remote")]
    public async Task<CliActionResult> PushAsync()
        => new(await _pushBranchCommand.ExecuteAsync());
}