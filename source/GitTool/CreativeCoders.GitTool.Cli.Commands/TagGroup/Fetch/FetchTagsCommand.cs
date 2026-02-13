using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Fetch;

[UsedImplicitly]
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
            fetchTagsCommand.Execute(new FetchTagsCommandOptions
            {
                RemoteName = remote.Name,
                Prune = options.Prune
            });
        }

        _ansiConsole.MarkupLine("Tags fetched successfully.".ToSuccessMarkup());

        return Task.FromResult(CommandResult.Success);
    }
}
