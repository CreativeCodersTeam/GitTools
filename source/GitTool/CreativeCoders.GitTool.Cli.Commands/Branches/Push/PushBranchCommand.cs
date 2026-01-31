using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using JetBrains.Annotations;
using IGitToolPushCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPushCommand;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.Push;

[UsedImplicitly]
[CliCommand([BranchesCommandGroup.Name, "push"], Description = "Pushes the current branch to remote")]
public class PushBranchCommand(IGitToolPushCommand pushCommand, IGitRepository gitRepository)
    : ICliCommand<PushBranchOptions>
{
    private readonly IGitToolPushCommand _pushCommand = Ensure.NotNull(pushCommand);

    public async Task<CommandResult> ExecuteAsync(PushBranchOptions options)
    {
        return await _pushCommand
            .ExecuteAsync(gitRepository, options.CreateRemoteBranchIfNotExists, options.ConfirmPush, options.Verbose)
            .ConfigureAwait(false);
    }
}
