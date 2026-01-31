using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Output;
using JetBrains.Annotations;
using Spectre.Console;
using IGitToolPullCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPullCommand;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.Update;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "update"],
    Description = "Update all permanent local branches by pulling from remote branches")]
public class UpdateBranchesCommand(
    IRepositoryConfigurations repositoryConfigurations,
    IGitToolPullCommand pullCommand,
    IAnsiConsole ansiConsole,
    ICml cml,
    IGitRepository gitRepository)
    : ICliCommand<UpdateBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly ICml _cml = Ensure.NotNull(cml);

    private readonly IGitToolPullCommand _pullCommand = Ensure.NotNull(pullCommand);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    public async Task<CommandResult> ExecuteAsync(UpdateBranchesOptions options)
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

        return CommandResult.Success;
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
