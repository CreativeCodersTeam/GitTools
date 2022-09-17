using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Cli.Actions.Exceptions;
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
        var mainBranch = gitRepository.Branches.LocalMainBranch;

        if (mainBranch == null || mainBranch.Commits == null)
        {
            throw new CliActionException("No main branch found");
        }

        //if (versionTag != null)
        //{
        //    versionTag.PeeledTargetCommit();

        //    _console.WriteMarkupLine($"Changelog for version {versionTag.Name.Friendly}");

        //    if (versionTag.TargetSha != gitRepository.Head.Tip?.Sha)
        //    {
        //        _console.WriteMarkupLine("New commits");
        //    }
        //}

        return CreateChangeLogLines(mainBranch.Commits);
    }

    private static IEnumerable<string> CreateChangeLogLines(IEnumerable<IGitCommit> commits)
    {
        return commits
            .Select(x => $"- {x.Message}");
    }
}
