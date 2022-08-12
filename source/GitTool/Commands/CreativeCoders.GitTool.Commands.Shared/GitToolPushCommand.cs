using System.Globalization;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
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
            .OnPushStatusError(OnGitPushStatusError)
            .OnUnPushedCommits(OnGitUnPushedCommits);

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

    private void OnGitUnPushedCommits(IEnumerable<IGitCommit> commits)
    {
        _ansiConsole.WriteLine("Commits to push to remote:");

        var maxMessageWidth = _ansiConsole.Profile.Width - 60;

        var commitsTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn("When")
            .AddColumn("Message", x => x.Width(maxMessageWidth).NoWrap())
            .AddColumn("Author")
            .AddColumn("Sha", x => x.Width(15).NoWrap());

        commits.ForEach(x =>
        {
            var when = x.Author.When.LocalDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat);

            var whenColumn = new Markup($"[italic teal]{when}[/]");

            var shaColumn = new Markup($"[silver]{x.Sha}[/]")
            {
                Overflow = Overflow.Ellipsis
            };

            var message = x.Message.ReplaceLineEndings(" ");

            message = message.Length < maxMessageWidth
                ? message
                : message[..maxMessageWidth];

            var messageColumn = new Markup($"[bold]{message}[/]")
            {
                Overflow = Overflow.Ellipsis
            };

            var authorColumn = new Markup($"[italic green]{x.Author.Name}[/]");

            commitsTable
                .AddRow(whenColumn, messageColumn, authorColumn, shaColumn);
        });

        _ansiConsole.Write(commitsTable);

        _ansiConsole.WriteLine();
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
