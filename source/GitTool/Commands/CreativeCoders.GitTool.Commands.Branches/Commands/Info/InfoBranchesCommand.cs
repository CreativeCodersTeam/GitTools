using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

[UsedImplicitly]
public class InfoBranchesCommand : IGitToolCommandWithOptions<InfoBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole;

    public InfoBranchesCommand(IAnsiConsole ansiConsole)
    {
        _ansiConsole = Ensure.NotNull(ansiConsole);
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, InfoBranchesOptions options)
    {
        _ansiConsole.WriteLine("Branch info");
        _ansiConsole.WriteLine();

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

        _ansiConsole.WriteLine();
        _ansiConsole.WriteLine("Last commits");

        var commits = gitRepository.Head.Commits?.Take(options.CommitLogCount);

        if (commits != null)
        {
            _ansiConsole.PrintCommitLog(commits);
        }

        _ansiConsole.WriteLine();

        return Task.FromResult(0);
    }
}
