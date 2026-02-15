using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.GitTool.Base.Versioning;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.ListVersions;

[CliCommand([ReleaseCommandGroup.Name, "list-versions"], Description = "Lists version tags")]
public class ListVersionsCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository)
    : ICliCommand<ListVersionsOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public Task<CommandResult> ExecuteAsync(ListVersionsOptions options)
    {
        _ansiConsole.WriteLines("Version tags:", string.Empty);

        _gitRepository.FetchAllTags("origin");

        var versionTags = _gitRepository.GetVersionTags();

        var versionComparer = new VersionComparer();

        var orderedVersionTags = options.SortDescending
            ? versionTags.OrderByDescending(x => x.Version, versionComparer)
            : versionTags.OrderBy(x => x.Version, versionComparer);

        foreach (var versionTag in orderedVersionTags)
        {
            _ansiConsole.WriteLine($"- {versionTag.Version}: {versionTag.Tag.Name.Friendly}");
        }

        return Task.FromResult(CommandResult.Success);
    }
}
