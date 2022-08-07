using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public class PullBranchCommand : IGitToolCommandWithOptions<PullBranchOptions>
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IGitToolPullCommand _pullCommand;

    public PullBranchCommand(IGitRepositoryFactory gitRepositoryFactory, IGitToolPullCommand pullCommand)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _pullCommand = Ensure.NotNull(pullCommand, nameof(pullCommand));
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, PullBranchOptions options)
    {
        return await _pullCommand.ExecuteAsync(gitRepository, options.Verbose).ConfigureAwait(false);
    }
}
