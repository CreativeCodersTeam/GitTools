using System.Globalization;
using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.GitTool.Cli.Commands.Shared;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.List;

[UsedImplicitly]
[CliCommand([TagCommandGroup.Name, "list"], Description = "Lists all tags")]
public class ListTagsCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository) : ICliCommand<ListTagsOptions>
{
    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    public Task<CommandResult> ExecuteAsync(ListTagsOptions options)
    {
        _ansiConsole.WriteLines("List all tags:", string.Empty);

        var tags = _gitRepository.Tags.OrderByDescending(x =>
            x.TargetCommit?.Author.When ?? DateTimeOffset.MinValue);

        options.ShowExtendedInformation
            .If(() => ShowExtendedTable(tags))
            .Else(() => ShowSimpleList(tags));

        return Task.FromResult(CommandResult.Success);
    }

    private void ShowSimpleList(IEnumerable<IGitTag> tags)
    {
        foreach (var tag in tags)
        {
            _ansiConsole.WriteLine($"- {tag.Name.Friendly}");
        }
    }

    private void ShowExtendedTable(IEnumerable<IGitTag> tags)
    {
        _ansiConsole.PrintTable(tags, [
            new TableColumnDef<IGitTag>(x => x.Name.Friendly, "Name"),
            new TableColumnDef<IGitTag>(x =>
            {
                var commit = x.TargetCommit;
                return commit == null
                    ? string.Empty
                    : commit.Author.When.ToString(
                        $"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern}");
            }, "Date of commit"),
            new TableColumnDef<IGitTag>(x =>
            {
                var commit = x.TargetCommit;
                return commit == null
                    ? string.Empty
                    : $"{commit.Author.Name} ({commit.Author.Email})";
            }, "Committer")
        ]);
    }
}
