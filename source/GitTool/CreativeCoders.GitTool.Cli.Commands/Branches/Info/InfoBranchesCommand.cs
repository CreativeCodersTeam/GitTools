using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.Info;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "info"], Description = "Shows information about the current branch")]
public class InfoBranchesCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository)
    : ICliCommand<InfoBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public Task<CommandResult> ExecuteAsync(InfoBranchesOptions options)
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

        return Task.FromResult(CommandResult.Success);
    }
}
