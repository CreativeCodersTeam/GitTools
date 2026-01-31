using System.Globalization;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Commits;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Shared;

public static class AnsiConsoleExtensions
{
    public static IAnsiConsole PrintCommitLog(this IAnsiConsole ansiConsole, IEnumerable<IGitCommit> commitLogs)
    {
        var maxMessageWidth = ansiConsole.Profile.Width - 60;

        var commitsTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn("When")
            .AddColumn("Message", x => x.Width(maxMessageWidth).NoWrap())
            .AddColumn("Author")
            .AddColumn("Sha", x => x.Width(15).NoWrap());

        commitLogs.ForEach(x =>
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

        ansiConsole.Write(commitsTable);

        return ansiConsole;
    }
}
