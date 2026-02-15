using CreativeCoders.Cli.Core;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
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

        var localTagExists =
            _gitRepository.Tags.Any(x => x.Name.Friendly == options.TagName || x.Name.Canonical == options.TagName);

        if (!localTagExists && !options.DeleteOnRemote)
        {
            throw new CliCommandAbortException($"Tag '{options.TagName}' not found", ReturnCodes.LocalTagNotFound);
        }

        if (localTagExists)
        {
            _gitRepository.Tags.DeleteTag(options.TagName);
            _ansiConsole.MarkupLine($"Tag '{options.TagName}' deleted successfully".ToSuccessMarkup());
        }
        else
        {
            _ansiConsole.MarkupLine(
                $"Skip deleting local tag '{options.TagName}' because it does not exist locally."
                    .ToInfoMarkup());
        }

        if (options.DeleteOnRemote)
        {
            _ansiConsole.WriteLine("Deleting tag on remote...");

            _gitRepository.Tags.DeleteRemoteTag(options.TagName);

            _ansiConsole.MarkupLine($"Tag '{options.TagName}' deleted successfully on remote".ToSuccessMarkup());
        }

        return Task.FromResult(CommandResult.Success);
    }
}
