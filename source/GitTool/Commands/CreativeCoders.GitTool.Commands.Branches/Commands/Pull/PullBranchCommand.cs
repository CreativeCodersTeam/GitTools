using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public class PullBranchCommand : IPullBranchCommand
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    public PullBranchCommand(IGitRepositoryFactory gitRepositoryFactory)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
    }

    public Task<int> ExecuteAsync()
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        gitRepository.Pull();

        return Task.FromResult(0);
    }
}
