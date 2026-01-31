using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.SysConsole.Core.Abstractions;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.List;

[UsedImplicitly]
[CliCommand([BranchCommandGroup.Name, "list"], Description = "Lists all branches")]
public class ListBranchesCommand(ISysConsole sysConsole, IGitRepository gitRepository)
    : ICliCommand<ListBranchesOptions>
{
    private readonly ISysConsole _sysConsole = Ensure.NotNull(sysConsole);

    private void PrintBranch(IGitBranch branch, int column0Width, int column1Width)
    {
        var isHead = branch.Equals(gitRepository.Head);

        _sysConsole.Write(isHead ? " * " : "   ");

        _sysConsole.ForegroundColor = isHead ? ConsoleColor.Green : ConsoleColor.Yellow;

        _sysConsole
            .Write(branch.Name.Friendly.PadRight(column0Width))
            .ResetColor();

        _sysConsole.Write($" ({branch.Name.Canonical.PadRight(column1Width)}) -> ");

        _sysConsole.ForegroundColor = ConsoleColor.DarkMagenta;

        _sysConsole
            .WriteLine(branch.TrackedBranch?.Name.Canonical)
            .ResetColor();
    }

    public Task<CommandResult> ExecuteAsync(ListBranchesOptions options)
    {
        _sysConsole
            .WriteLine()
            .WriteLine("List all branches:")
            .WriteLine();

        var branches = gitRepository
            .Branches
            .Where(x =>
                options.Location == BranchLocation.All
                || (options.Location == BranchLocation.Local && !x.IsRemote)
                || (options.Location == BranchLocation.Remote && x.IsRemote))
            .ToArray();

        var column0Width = branches.Select(x => x.Name.Friendly.Length).Max();
        var column1Width = branches.Select(x => x.Name.Canonical.Length).Max();

        _sysConsole.WriteLine(
            $"   {"Friendly".PadRight(column0Width)}  {"Canonical".PadRight(column1Width)}     Tracked remote branch");

        // ReSharper disable once AccessToDisposedClosure
        branches.ForEach(branch => PrintBranch(branch, column0Width, column1Width));

        _sysConsole.WriteLine();

        return Task.FromResult(CommandResult.Success);
    }
}
