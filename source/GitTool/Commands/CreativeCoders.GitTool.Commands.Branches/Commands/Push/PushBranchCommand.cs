using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

[UsedImplicitly]
public class PushBranchCommand : IGitToolCommandWithOptions<PushBranchOptions>
{
    private readonly IGitToolPushCommand _pushCommand;

    public PushBranchCommand(IGitToolPushCommand pushCommand)
    {
        _pushCommand = Ensure.NotNull(pushCommand);
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, PushBranchOptions options)
    {
        return await _pushCommand
            .ExecuteAsync(gitRepository, options.CreateRemoteBranchIfNotExists, options.ConfirmPush, options.Verbose)
            .ConfigureAwait(false);
    }
}
