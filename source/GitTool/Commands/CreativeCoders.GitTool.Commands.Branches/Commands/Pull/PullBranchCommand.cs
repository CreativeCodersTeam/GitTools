using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public class PullBranchCommand : IPullBranchCommand
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IGitToolPullCommand _pullCommand;

    public PullBranchCommand(IGitRepositoryFactory gitRepositoryFactory, IGitToolPullCommand pullCommand)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _pullCommand = Ensure.NotNull(pullCommand, nameof(pullCommand));
    }

    public async Task<int> ExecuteAsync(PullBranchOptions options)
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        return await _pullCommand.ExecuteAsync(gitRepository, options.Verbose).ConfigureAwait(false);
    }
}
