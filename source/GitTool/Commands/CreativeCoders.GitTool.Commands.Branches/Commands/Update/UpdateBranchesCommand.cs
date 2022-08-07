using System.Collections.Generic;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Update;

public class UpdateBranchesCommand : IUpdateBranchesCommand
{
    private readonly ICml _cml;

    private readonly IAnsiConsole _ansiConsole;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IGitToolPullCommand _pullCommand;

    public UpdateBranchesCommand(IGitRepositoryFactory gitRepositoryFactory,
        IRepositoryConfigurations repositoryConfigurations, IGitToolPullCommand pullCommand,
        IAnsiConsole ansiConsole, ICml cml)
    {
        _cml = Ensure.NotNull(cml, nameof(cml));
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations, nameof(repositoryConfigurations));
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _pullCommand = Ensure.NotNull(pullCommand, nameof(pullCommand));
    }

    public async Task<int> ExecuteAsync(UpdateBranchesOptions options)
    {
        Ensure.NotNull(options, nameof(options));

        _ansiConsole
            .EmptyLine()
            .WriteMarkupLine(_cml.Caption("Update permanent local branches"))
            .EmptyLine();

        using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        var currentBranch = repository.Head;

        var updateBranchNames = new List<string>
        {
            "production",
            GitBranchNames.Local.GetFriendlyName(repository.Info.MainBranch)
        };

        if (configuration.HasDevelopBranch)
        {
            updateBranchNames.Add(configuration.DevelopBranch);
        }

        _ansiConsole.WriteLine("Fetch prune to remove remote branch refs if already deleted");

        repository.FetchPruneFromOrigin();

        await UpdateBranchesAsync(repository, updateBranchNames).ConfigureAwait(false);

        if (!repository.Head.Equals(currentBranch))
        {
            _ansiConsole.WriteMarkupLine(
                $"Switch back to working branch {_cml.HighLight($"'{currentBranch.Name.Friendly}'")}");

            repository.CheckOut(currentBranch.Name.Friendly);
        }

        _ansiConsole.EmptyLine();

        return 0;
    }

    private async Task UpdateBranchesAsync(IGitRepository repository, IEnumerable<string> updateBranchNames)
    {
        await updateBranchNames
            .ForEachAsync(async branchName =>
            {
                if (repository.Branches[branchName] == null)
                {
                    return;
                }

                _ansiConsole.WriteMarkupLine($"Update local branch {_cml.HighLight($"'{branchName}'")}");

                repository.CheckOut(branchName);

                await _pullCommand.ExecuteAsync(repository).ConfigureAwait(false);
            })
            .ConfigureAwait(false);
    }
}