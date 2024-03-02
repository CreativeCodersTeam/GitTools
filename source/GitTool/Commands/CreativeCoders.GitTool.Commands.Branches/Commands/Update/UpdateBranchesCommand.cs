using System.Collections.Generic;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Update;

[UsedImplicitly]
public class UpdateBranchesCommand : IGitToolCommandWithOptions<UpdateBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly ICml _cml;

    private readonly IGitToolPullCommand _pullCommand;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    public UpdateBranchesCommand(IRepositoryConfigurations repositoryConfigurations, IGitToolPullCommand pullCommand,
        IAnsiConsole ansiConsole, ICml cml)
    {
        _cml = Ensure.NotNull(cml);
        _ansiConsole = Ensure.NotNull(ansiConsole);
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);
        _pullCommand = Ensure.NotNull(pullCommand);
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, UpdateBranchesOptions options)
    {
        Ensure.NotNull(options);

        _ansiConsole
            .EmptyLine()
            .WriteMarkupLine(_cml.Caption("Update permanent local branches"))
            .EmptyLine();

        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        var currentBranch = gitRepository.Head;

        var updateBranchNames = new List<string>
        {
            "production",
            GitBranchNames.Local.GetFriendlyName(gitRepository.Info.MainBranch)
        };

        if (configuration.HasDevelopBranch)
        {
            updateBranchNames.Add(configuration.DevelopBranch);
        }

        if (!options.SkipFetchPrune)
        {
            _ansiConsole.WriteLine("Fetch prune to remove remote branch refs if already deleted");

            gitRepository.FetchPruneFromOrigin();
        }

        await UpdateBranchesAsync(gitRepository, updateBranchNames).ConfigureAwait(false);

        if (!gitRepository.Head.Equals(currentBranch))
        {
            _ansiConsole.WriteMarkupLine(
                $"Switch back to working branch {_cml.HighLight($"'{currentBranch.Name.Friendly}'")}");

            gitRepository.Branches.CheckOut(currentBranch.Name.Friendly);
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

                repository.Branches.CheckOut(branchName);

                await _pullCommand.ExecuteAsync(repository).ConfigureAwait(false);
            })
            .ConfigureAwait(false);
    }
}
