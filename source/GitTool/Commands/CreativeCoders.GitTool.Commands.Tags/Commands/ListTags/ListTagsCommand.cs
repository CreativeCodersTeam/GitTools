using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Exceptions;
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
        var tags = GetTags(gitRepository, options);

        tags.ForEach(PrintTag);

        return Task.FromResult(0);
    }

    private static IEnumerable<IGitTag> GetTags(IGitRepository gitRepository, ListTagsOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Branch))
        {
            return gitRepository.Tags;
        }

        var branch = gitRepository.Branches[options.Branch];

        if (branch == null)
        {
            throw new GitBranchNotExistsException();
        }

        return gitRepository.Tags.GetAllTagsForBranch(branch);
    }

    public void PrintTag(IGitTag tag)
    {
        var targetCommitMessage = tag.PeeledTargetCommit()?.Message.Split(Env.NewLine).FirstOrDefault();

        _console.WriteMarkupLine($"{tag.Name.Friendly} -> {tag.TargetSha}  {targetCommitMessage}");
    }
}
