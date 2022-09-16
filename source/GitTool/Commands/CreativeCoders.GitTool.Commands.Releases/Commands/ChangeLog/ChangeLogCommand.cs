using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.ChangeLog;

public class ChangeLogCommand : IGitToolCommandWithOptions<ChangeLogOptions>
{
    private readonly IAnsiConsole _console;

    public ChangeLogCommand(IAnsiConsole console)
    {
        _console = Ensure.NotNull(console, nameof(console));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, ChangeLogOptions options)
    {
        var commitLines = CreateChangeLogLines(gitRepository, options);

        commitLines.ForEach(x => _console.WriteLine(x));

        return Task.FromResult(0);
    }

    private IEnumerable<string> CreateChangeLogLines(IGitRepository gitRepository, ChangeLogOptions options)
    {
        var tag = gitRepository.Tags.Reverse().FirstOrDefault();

        if (tag != null)
        {
            _console.WriteMarkupLine(tag.Name.Friendly);

            if (tag.TargetSha != gitRepository.Head.Tip.Sha)
            {
                _console.WriteMarkupLine("New commits");
            }
        }
        else
        {
            return CreateChangeLogLines(gitRepository.Commits);
        }

        return Array.Empty<string>();
    }

    private static IEnumerable<string> CreateChangeLogLines(IEnumerable<IGitCommit> commits)
    {
        return commits
            .Select(x => $"- {x.Message}");
    }
}
