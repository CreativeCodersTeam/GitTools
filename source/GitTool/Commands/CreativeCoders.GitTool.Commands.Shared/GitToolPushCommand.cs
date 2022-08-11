using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Pushes;
using CreativeCoders.GitTool.Base.Output;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Shared;

public class GitToolPushCommand : IGitToolPushCommand
{
    private readonly IAnsiConsole _ansiConsole;

    private readonly ICml _cml;

    public GitToolPushCommand(IAnsiConsole ansiConsole, ICml cml)
    {
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
        _cml = Ensure.NotNull(cml, nameof(cml));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists)
        => ExecuteAsync(gitRepository, createRemoteIsNotExists, false);

    public Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists, bool verbose)
    {
        _ansiConsole
            .WriteMarkupLine("Push updates to remote origin")
            .WriteLine();

        var pushCommand = gitRepository.Commands
            .CreatePushCommand()
            .CreateRemoteBranchIfNotExists(createRemoteIsNotExists)
            .OnNegotiationCompletedBeforePush(OnGitNegotiationCompletedBeforePush)
            .OnPushStatusError(OnGitPushStatusError);

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
