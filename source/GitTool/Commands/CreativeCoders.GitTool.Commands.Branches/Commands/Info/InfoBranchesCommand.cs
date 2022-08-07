using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Core.Abstractions;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

public class InfoBranchesCommand : IGitToolCommandWithOptions<InfoBranchesOptions>
{
    private readonly ISysConsole _sysConsole;

    private readonly IAnsiConsole _ansiConsole;

    public InfoBranchesCommand(IAnsiConsole ansiConsole, ISysConsole sysConsole)
    {
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, InfoBranchesOptions options)
    {
        _sysConsole
            .WriteLine("Branch info")
            .WriteLine();

        var table = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn("Name")
            .AddColumn("Value")
            .AddRow("Current branch:", gitRepository.Head.Name.Canonical)
            .AddRow("Last Commit:", gitRepository.Head.Tip?.Sha ?? string.Empty);

        if (gitRepository.Head.TrackedBranch != null)
        {
            table.AddRow("Last tracked commit:", gitRepository.Head.TrackedBranch?.Tip?.Sha ?? string.Empty);
        }

        _ansiConsole.Write(table);

        _sysConsole
            .WriteLine()
            .WriteLine("Last commits");

        var maxMessageWidth = _ansiConsole.Profile.Width - 60;

        var commitsTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn("When")
            .AddColumn("Message", x => x.Width(maxMessageWidth).NoWrap())
            .AddColumn("Author")
            .AddColumn("Sha", x => x.Width(15).NoWrap());

        gitRepository.Head.Commits?.Take(options.CommitLogCount).ForEach(x =>
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
        
        _sysConsole.WriteLine();

        return Task.FromResult(0);
    }
}
