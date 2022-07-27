using System;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Merges;
using CreativeCoders.GitTool.Base.Output;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public class PullBranchCommand : IPullBranchCommand
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IAnsiConsole _ansiConsole;

    private readonly ICml _cml;

    public PullBranchCommand(IGitRepositoryFactory gitRepositoryFactory, IAnsiConsole ansiConsole, ICml cml)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
        _cml = Ensure.NotNull(cml, nameof(cml));
    }

    public Task<int> ExecuteAsync()
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        _ansiConsole
            .WriteMarkupLine($"Pull branch '{_cml.Text(gitRepository.Head.Name.Friendly)}' updates from remote origin")
            .WriteLine();

        var pullCommand = gitRepository.Commands.CreatePullCommand();

        var mergeResult = pullCommand
            .CheckoutNotify(CheckoutNotify)
            .Run();

        switch (mergeResult.MergeStatus)
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
                throw new ArgumentOutOfRangeException();
        }

        _ansiConsole.WriteLine();

        return Task.FromResult(0);
    }

    private void CheckoutNotify(string path, GitCheckoutNotifyFlags notifyFlags)
    {
        _ansiConsole.MarkupLine($"{_cml.HighLight(notifyFlags.ToString())}: {path}");
    }
}
