using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.SysConsole.Core.Abstractions;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

public class InfoBranchesCommand : IInfoBranchesCommand
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly ISysConsole _sysConsole;

    private readonly IAnsiConsole _ansiConsole;

    public InfoBranchesCommand(IGitRepositoryFactory gitRepositoryFactory, IAnsiConsole ansiConsole, ISysConsole sysConsole)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
    }

    public Task<int> ExecuteAsync()
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

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

        var maxMessageWidth = _ansiConsole.Profile.Width - 30;

        var commitsTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn("Sha", x => x.Width(10).NoWrap())
            .AddColumn("Message", x => x.Width(maxMessageWidth).NoWrap())
            .AddColumn("Author");

        gitRepository.Head.Commits?.Take(10).ForEach(x =>
        {
            var shaColumn = new Markup($"[italic green]{x.Sha}[/]")
            {
                Overflow = Overflow.Ellipsis
            };

            var message = x.Message.ReplaceLineEndings(" ");

            message = message.Length < maxMessageWidth
                ? message
                : message.Substring(0, maxMessageWidth);

            var messageColumn = new Markup($"[bold]{message}[/]")
            {
                Overflow = Overflow.Ellipsis
            };

            var authorColumn = new Markup($"[italic green]{x.Author.Name}[/]");

            commitsTable
                .AddRow(shaColumn, messageColumn, authorColumn);
        });

        _ansiConsole.Write(commitsTable);
        
        _sysConsole.WriteLine();

        return Task.FromResult(0);
    }
}
