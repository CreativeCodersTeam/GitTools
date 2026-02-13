using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Create;

[UsedImplicitly]
[CliCommand([TagCommandGroup.Name, "create"], Description = "Creates a new tag")]
public class CreateTagCommand(IAnsiConsole ansiConsole, IGitRepository gitRepository) : ICliCommand<CreateTagOptions>
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public Task<CommandResult> ExecuteAsync(CreateTagOptions options)
    {
        _ansiConsole.WriteLine($"Creating tag '{options.TagName}'...");

        var tag = CreateTag(options);

        _ansiConsole.MarkupLines(
            $"Tag '{tag.Name}' created.".ToSuccessMarkup(),
            string.Empty,
            $"Target commit: {tag.TargetCommit?.Id.Sha ?? "[none]"}".ToEscapedMarkup());

        if (options.PushAfterCreate)
        {
            _ansiConsole.WriteLines(
                string.Empty,
                "Pushing tag after creation");

            _gitRepository.Tags.PushTag(tag);

            _ansiConsole.MarkupLine("Tag pushed.".ToSuccessMarkup());
        }

        return Task.FromResult(CommandResult.Success);
    }

    private IGitTag CreateTag(CreateTagOptions options)
    {
        return string.IsNullOrWhiteSpace(options.Message)
            ? _gitRepository.Tags.CreateTag(options.TagName, options.Objectish)
            : _gitRepository.Tags.CreateTagWithMessage(options.TagName, options.Message, options.Objectish);
    }
}
