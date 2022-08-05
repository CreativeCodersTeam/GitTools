using System;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Merges;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared;
using Spectre.Console;

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

    public async Task<int> ExecuteAsync()
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        return await _pullCommand.ExecuteAsync(gitRepository).ConfigureAwait(false);
    }
}
