using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Tags.Commands.ListTags;

public class ListTagsCommand : IGitToolCommandWithOptions<ListTagsOptions>
{
    private readonly IAnsiConsole _console;

    public ListTagsCommand(IAnsiConsole console)
    {
        _console = Ensure.NotNull(console, nameof(console));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, ListTagsOptions options)
    {
        gitRepository.Tags.ForEach(PrintTag);

        return Task.FromResult(0);
    }

    public void PrintTag(IGitTag tag)
    {
        var targetCommitMessage = tag.PeeledTargetCommit()?.Message.Split(Env.NewLine).FirstOrDefault();

        _console.WriteMarkupLine($"{tag.Name.Friendly} -> {tag.TargetSha}  {targetCommitMessage}");
    }
}
