using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

public class PushBranchCommand : IGitToolCommandWithOptions<PushBranchOptions>
{
    private readonly IGitToolPushCommand _pushCommand;

    public PushBranchCommand(IGitToolPushCommand pushCommand)
    {
        _pushCommand = Ensure.NotNull(pushCommand, nameof(pushCommand));
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, PushBranchOptions options)
    {
        return await _pushCommand
            .ExecuteAsync(gitRepository, options.CreateRemoteBranchIfNotExists, options.Verbose)
            .ConfigureAwait(false);
    }
}
