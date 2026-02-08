using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Fetch;

[CliCommand([TagCommandGroup.Name, "fetch"], Description = "Fetch tags from remote repository")]
public class FetchTagsCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository) : ICliCommand<FetchTagsOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public Task<CommandResult> ExecuteAsync(FetchTagsOptions options)
    {
        _ansiConsole.WriteLines("Fetch tags from remote repository", string.Empty);

        var remotes = _gitRepository.Remotes.ToList();

        if (remotes.Count == 0)
        {
            _ansiConsole.WriteLine("No remote repository found.");
            return Task.FromResult(CommandResult.Success);
        }

        var fetchTagsCommand = _gitRepository.Commands.CreateFetchTagsCommand();

        foreach (var remote in remotes)
        {
            _ansiConsole.WriteLine($"Fetch tags from remote '{remote.Name}' ({remote.Url})...");
            fetchTagsCommand.Execute(remote.Name);
        }

        _ansiConsole.WriteLine("Tags fetched successfully.");

        return Task.FromResult(CommandResult.Success);
    }
}
