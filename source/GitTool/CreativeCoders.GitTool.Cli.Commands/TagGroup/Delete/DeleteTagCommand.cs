using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Delete;

[UsedImplicitly]
[CliCommand([TagCommandGroup.Name, "delete"], Description = "Deletes a tag from the repository")]
public class DeleteTagCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository) : ICliCommand<DeleteTagOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public Task<CommandResult> ExecuteAsync(DeleteTagOptions options)
    {
        _ansiConsole.WriteLine($"Deleting tag '{options.TagName}'...");

        _gitRepository.Tags.DeleteTag(options.TagName);

        _ansiConsole.MarkupLine($"Tag '{options.TagName}' deleted successfully".ToSuccessMarkup());

        if (options.DeleteOnRemote)
        {
            _ansiConsole.WriteLine("Deleting tag on remote...");

            _gitRepository.Tags.DeleteRemoteTag(options.TagName);

            _ansiConsole.MarkupLine($"Tag '{options.TagName}' deleted successfully on remote".ToSuccessMarkup());
        }

        return Task.FromResult(CommandResult.Success);
    }
}
