using CreativeCoders.Cli.Core;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "remove-orphaned"],
    Description = "Removes local branches which have no counterpart on the remote")]
public class RemoveOrphanedLocalBranchesCommand(
    IAnsiConsole ansiConsole,
    ICml cml,
    IGitRepository gitRepository,
    IOrphanedLocalBranchesProvider orphanedLocalBranchesProvider)
    : ICliCommand<RemoveOrphanedLocalBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly ICml _cml = Ensure.NotNull(cml);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    private readonly IOrphanedLocalBranchesProvider _orphanedLocalBranchesProvider =
        Ensure.NotNull(orphanedLocalBranchesProvider);

    public async Task<CommandResult> ExecuteAsync(RemoveOrphanedLocalBranchesOptions options)
    {
        Ensure.NotNull(options);

        _ansiConsole
            .WriteMarkupLine(_cml.Caption("Remove orphaned local branches"))
            .EmptyLine();

        var fetchPruneFailed = false;

        if (!options.SkipFetchPrune)
        {
            _ansiConsole.WriteLine("Fetch prune to remove remote branch refs if already deleted");

            try
            {
                _gitRepository.FetchPruneFromOrigin();
            }
            catch (GitException e)
            {
                fetchPruneFailed = true;

                _ansiConsole.MarkupLine(
                    $"Fetch prune failed: {e.Message.EscapeMarkup()}. Continuing with the local branch state."
                        .ToWarningMarkup());
            }
        }

        var orphanedBranches = _orphanedLocalBranchesProvider.GetOrphanedLocalBranches();

        if (orphanedBranches.Count == 0)
        {
            _ansiConsole.MarkupLine("No orphaned local branches found".ToInfoMarkup());

            return CommandResult.Success;
        }

        // A stale remote state can make branches look orphaned, so '--all' only skips the selection
        // if the branches were determined from an up to date remote state
        var requiresSelection = !options.All || fetchPruneFailed;

        // The interactive check runs after the scan: a repo with no orphaned branches succeeds
        // non-interactively, and only an actual selection needs a terminal.
        if (requiresSelection && !_ansiConsole.Profile.Capabilities.Interactive)
        {
            var abortMessage = options.All
                ? "The fetch prune failed, so the orphaned branches may be based on a stale remote "
                  + "state. Deleting them without selection needs an interactive terminal. "
                  + "Use option '--skip-fetch-prune' to accept the local branch state."
                : "Selecting branches needs an interactive terminal. Use option '--all' to delete "
                  + "all orphaned local branches without selection"
                  + (fetchPruneFailed
                      ? " and '--skip-fetch-prune' to accept the local branch state."
                      : ".");

            throw new CliCommandAbortException(abortMessage, ReturnCodes.NoInteractiveTerminal);
        }

        if (options.All && fetchPruneFailed)
        {
            _ansiConsole.MarkupLine(
                "Option '--all' is ignored because the fetch prune failed. Select the branches to delete."
                    .ToWarningMarkup());
        }

        var selectedBranches = requiresSelection
            ? await SelectBranchesAsync(orphanedBranches).ConfigureAwait(false)
            : orphanedBranches;

        if (selectedBranches.Count == 0)
        {
            _ansiConsole.MarkupLine("No branches selected for deletion".ToInfoMarkup());

            return CommandResult.Success;
        }

        return DeleteBranches(selectedBranches);
    }

    private async Task<IReadOnlyCollection<IGitBranch>> SelectBranchesAsync(
        IReadOnlyCollection<IGitBranch> orphanedBranches)
    {
        var prompt = new MultiSelectionPrompt<IGitBranch>()
            .Title("Select the local branches to delete")
            .NotRequired()
            .PageSize(20)
            .UseConverter(branch => branch.Name.Friendly.EscapeMarkup())
            .InstructionsText("Press <space> to toggle a branch, <enter> to accept the selection");

        foreach (var orphanedBranch in orphanedBranches)
        {
            prompt.AddChoice(orphanedBranch).Select();
        }

        return await _ansiConsole.PromptAsync(prompt).ConfigureAwait(false);
    }

    private CommandResult DeleteBranches(IReadOnlyCollection<IGitBranch> branches)
    {
        var failedBranchCount = 0;

        foreach (var branch in branches)
        {
            var branchName = branch.Name.Friendly;

            try
            {
                _gitRepository.Branches.DeleteLocalBranch(branchName);

                _ansiConsole.MarkupLine(
                    $"Local branch '{branchName.EscapeMarkup()}' deleted".ToSuccessMarkup());
            }
            catch (GitException e)
            {
                failedBranchCount++;

                _ansiConsole.MarkupLine(
                    $"Deleting local branch '{branchName.EscapeMarkup()}' failed: {e.Message.EscapeMarkup()}"
                        .ToErrorMarkup());
            }
        }

        var deletedBranchCount = branches.Count - failedBranchCount;

        var summary = $"{deletedBranchCount} of {branches.Count} local "
                      + $"{(branches.Count == 1 ? "branch" : "branches")} deleted";

        _ansiConsole
            .EmptyLine()
            .MarkupLine(failedBranchCount == 0
                ? summary.ToSuccessMarkup()
                : summary.ToWarningMarkup());

        return failedBranchCount == 0
            ? CommandResult.Success
            : ReturnCodes.DeleteLocalBranchFailed;
    }
}
