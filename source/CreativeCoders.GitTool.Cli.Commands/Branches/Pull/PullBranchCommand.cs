using System.Threading.Tasks;
using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.Pull;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "pull"], Description = "Pulls the current branch from remote")]
public class PullBranchCommand(IGitToolPullCommand pullCommand, IGitRepository gitRepository)
    : ICliCommand<PullBranchOptions>
{
    private readonly IGitToolPullCommand _pullCommand = Ensure.NotNull(pullCommand);

    public async Task<CommandResult> ExecuteAsync(PullBranchOptions options)
    {
        return await _pullCommand.ExecuteAsync(gitRepository, options.Verbose).ConfigureAwait(false);
    }
}
