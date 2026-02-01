using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using JetBrains.Annotations;
using IGitToolPullCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPullCommand;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.Pull;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "pull"], Description = "Pulls the current branch from remote")]
public class PullBranchCommand(IGitToolPullCommand pullCommand, IGitRepository gitRepository)
    : ICliCommand<PullBranchOptions>
{
    private readonly IGitToolPullCommand _pullCommand = Ensure.NotNull(pullCommand);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public async Task<CommandResult> ExecuteAsync(PullBranchOptions options)
    {
        return await _pullCommand.ExecuteAsync(_gitRepository, options.Verbose).ConfigureAwait(false);
    }
}
