using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
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

    private static IEnumerable<string> CreateChangeLogLines(IGitRepository gitRepository, ChangeLogOptions options)
    {
        var mainBranch = gitRepository.Branches.LocalMainBranch;

        if (mainBranch?.Commits == null)
        {
            throw new CliActionException("No main branch found");
        }

        return CreateChangeLogLines(GetCommits(gitRepository, mainBranch));
    }

    private static IEnumerable<IGitCommit> GetCommits(IGitRepository gitRepository, IGitBranch branch)
    {
        var tags = gitRepository.Tags.GetAllTagsForBranch(branch).Reverse().ToArray();

        if (tags.Length > 0)
        {
            var firstTag = tags.First();

            if (firstTag.TargetSha != branch.Commits?.First().Sha)
            {
                var commits = branch.Commits.TakeUntil(x => x.Sha == firstTag.TargetSha).ToArray();

                return commits.Take(commits.Length - 1);
            }

            if (tags.Length > 1)
            {

            }

            return Array.Empty<IGitCommit>();
        }

        return branch.Commits?.ToArray() ?? Array.Empty<IGitCommit>();
    }

    private static IEnumerable<string> CreateChangeLogLines(IEnumerable<IGitCommit> commits)
    {
        return commits
            .Select(x => $"- {FormatCommitMessage(x)}");
    }

    private static string FormatCommitMessage(IGitCommit commit)
    {
        var lines = commit.Message.Split("\n").Select(x => x.TrimEnd()).ToArray();

        for (var i = 1; i < lines.Length; i++)
        {
            lines[i] = $"  {lines[i]}";
        }

        return string.Join(Env.NewLine, lines);
    }
}
