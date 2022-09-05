using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Merges;
using CreativeCoders.GitTool.Base.Output;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Shared;

public class GitToolPullCommand : IGitToolPullCommand
{
    private readonly IAnsiConsole _ansiConsole;

    private readonly ICml _cml;

    public GitToolPullCommand(IAnsiConsole ansiConsole, ICml cml)
    {
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
        _cml = Ensure.NotNull(cml, nameof(cml));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository)
    {
        return ExecuteAsync(gitRepository, false);
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, bool verbose)
    {
        _ansiConsole
            .WriteMarkupLine($"Pull branch '{_cml.Text(gitRepository.Head.Name.Friendly)}' updates from remote origin")
            .WriteLine();

        var pullCommand = gitRepository.Commands.CreatePullCommand();

        if (verbose)
        {
            pullCommand = pullCommand
                .OnCheckoutProgress(CheckoutProgress);
        }

        var mergeResult = pullCommand
            .OnCheckoutNotify((path, flags) => CheckoutNotify(path, flags, verbose))
            .Run();

        PrintMergeResultStatus(mergeResult.MergeStatus);

        _ansiConsole.WriteLine();

        return Task.FromResult(0);
    }

    private void CheckoutProgress(string path, int completedSteps, int totalSteps)
    {
        _ansiConsole.MarkupLine($"{_cml.HighLight(path)} {completedSteps/totalSteps}");
    }

    private void PrintMergeResultStatus(GitMergeStatus mergeResultStatus)
    {
        switch (mergeResultStatus)
        {
            case GitMergeStatus.UpToDate:
                _ansiConsole.WriteLine("Branch is already up to date");
                break;
            case GitMergeStatus.FastForward:
                _ansiConsole.WriteLine("Branch was merged fast forward");
                break;
            case GitMergeStatus.Conflicts:
                _ansiConsole.WriteLine("Merging failed with conflicts");
                break;
            case GitMergeStatus.NonFastForward:
                _ansiConsole.WriteLine("Changes were merged");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mergeResultStatus));
        }
    }

    private void CheckoutNotify(string path, GitCheckoutNotifyFlags notifyFlags, bool verbose)
    {
        if (!verbose && notifyFlags == GitCheckoutNotifyFlags.Ignored)
        {
            return;
        }

        _ansiConsole.MarkupLine($"{_cml.HighLight(notifyFlags.ToString())}: {path}");
    }
}
