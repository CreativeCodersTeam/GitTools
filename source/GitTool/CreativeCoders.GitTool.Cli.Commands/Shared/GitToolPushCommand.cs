using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Pushes;
using CreativeCoders.GitTool.Base.Output;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Shared;

public class GitToolPushCommand(IAnsiConsole ansiConsole, ICml cml) : IGitToolPushCommand
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly ICml _cml = Ensure.NotNull(cml);

    public Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists, bool confirmPush)
        => ExecuteAsync(gitRepository, createRemoteIsNotExists, confirmPush, false);

    public Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists, bool confirmPush,
        bool verbose)
    {
        _ansiConsole
            .WriteMarkupLine("Push updates to remote origin")
            .WriteLine();

        var pushCommand = gitRepository.Commands
            .CreatePushCommand()
            .Confirm(confirmPush)
            .CreateRemoteBranchIfNotExists(createRemoteIsNotExists)
            .OnNegotiationCompletedBeforePush(OnGitNegotiationCompletedBeforePush)
            .OnPushStatusError(OnGitPushStatusError)
            .OnUnPushedCommits(OnGitUnPushedCommits)
            .OnConfirm(DoConfirm);

        if (verbose)
        {
            pushCommand = pushCommand
                .OnTransferProgress(OnGitTransferProgress)
                .OnPackBuilderProgress(OnGitPackBuilderProgress);
        }

        pushCommand.Run();

        _ansiConsole.WriteLine();

        return Task.FromResult(0);
    }

    private bool DoConfirm()
    {
        _ansiConsole.WriteLine();
        _ansiConsole.MarkupLine("Push commits? (y/n)");

        var key = _ansiConsole.Input.ReadKey(false);

        return key is { KeyChar: 'y' };
    }

    private void OnGitUnPushedCommits(IEnumerable<IGitCommit> commits)
    {
        _ansiConsole.WriteLine("Commits to push to remote:");

        _ansiConsole
            .PrintCommitLog(commits)
            .WriteLine();
    }

    private void OnGitPackBuilderProgress(GitPackBuilderProgress packBuilderProgress)
    {
        _ansiConsole.MarkupLine(
            $"Build pack: {_cml.HighLight(packBuilderProgress.Stage.ToString())}  {packBuilderProgress.Current}/{packBuilderProgress.Total}");
    }

    private void OnGitTransferProgress(GitPushTransferProgress transferProgress)
    {
        _ansiConsole.MarkupLine(
            $"Transfer: {transferProgress.Current}/{transferProgress.Total}  {transferProgress.Bytes} bytes");
    }

    private void OnGitPushStatusError(GitPushStatusError error)
    {
        _ansiConsole.MarkupLine($"{_cml.HighLight(error.Reference)}: {_cml.Error(error.Message)}");
    }

    private void OnGitNegotiationCompletedBeforePush(IEnumerable<GitPushUpdate> updates)
    {
        updates
            .ForEach(x =>
                _ansiConsole.MarkupLine($"{_cml.Text(x.SourceRefName)} => {_cml.Text(x.DestinationRefName)}"));
    }
}
