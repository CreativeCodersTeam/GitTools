using System.Collections.Generic;
using System.Linq;
using System.Text;
using CreativeCoders.Git.Abstractions.Commits;

namespace CreativeCoders.GitTool.Base.PullRequestInfos;

public static class PullRequestBodyGenerator
{
    private const string SummaryHeader = "## Summary";
    private const string ChangesHeader = "## Changes";
    private const string NoChangesMessage = "No changes detected.";
    private const string BreakingChangeWarning = "⚠️ This branch contains **breaking changes**. ";
    private const string BreakingChangePrefix = "⚠️ **BREAKING**: ";

    public static string GenerateBody(IEnumerable<IGitCommit> commits)
    {
        var conventionalCommits = ParseConventionalCommits(commits);

        if (conventionalCommits.Count == 0)
        {
            return string.Empty;
        }

        var bodyBuilder = new StringBuilder();

        AppendSummary(bodyBuilder, conventionalCommits);
        AppendChanges(bodyBuilder, conventionalCommits);

        return bodyBuilder.ToString().TrimEnd();
    }

    private static List<ConventionalCommit> ParseConventionalCommits(IEnumerable<IGitCommit> commits)
    {
        return commits
            .Select(commit => ConventionalCommit.Parse(commit.Message))
            .Where(parsed => parsed != null)
            .Select(parsed => parsed!)
            .ToList();
    }

    private static void AppendSummary(StringBuilder bodyBuilder, List<ConventionalCommit> commits)
    {
        var summary = GenerateSummary(commits);
        bodyBuilder.AppendLine(summary);
        bodyBuilder.AppendLine();
    }

    private static void AppendChanges(StringBuilder bodyBuilder, List<ConventionalCommit> commits)
    {
        bodyBuilder.AppendLine(ChangesHeader);
        bodyBuilder.AppendLine();

        var groupedCommits = GroupAndOrderCommitsByType(commits);

        foreach (var group in groupedCommits)
        {
            AppendCommitGroup(bodyBuilder, group);
        }
    }

    private static IOrderedEnumerable<IGrouping<string, ConventionalCommit>> GroupAndOrderCommitsByType(
        List<ConventionalCommit> commits)
    {
        return commits
            .GroupBy(commit => commit.Type)
            .OrderBy(group => CommitTypeMetadata.GetMetadata(group.Key).Priority);
    }

    private static void AppendCommitGroup(StringBuilder bodyBuilder, IGrouping<string, ConventionalCommit> group)
    {
        var metadata = CommitTypeMetadata.GetMetadata(group.Key);

        bodyBuilder.AppendLine($"### {metadata.Header}");
        bodyBuilder.AppendLine();

        foreach (var commit in group)
        {
            var commitLine = FormatCommitLine(commit);
            bodyBuilder.AppendLine(commitLine);
        }

        bodyBuilder.AppendLine();
    }

    private static string FormatCommitLine(ConventionalCommit commit)
    {
        var scope = FormatScope(commit.Scope);
        var breaking = FormatBreakingChange(commit.IsBreakingChange);

        return $"- {breaking}{scope}{commit.Description}";
    }

    private static string FormatScope(string? scope)
    {
        return scope != null ? $"**{scope}**: " : string.Empty;
    }

    private static string FormatBreakingChange(bool isBreakingChange)
    {
        return isBreakingChange ? BreakingChangePrefix : string.Empty;
    }

    private static string GenerateSummary(List<ConventionalCommit> commits)
    {
        if (commits.Count == 0)
        {
            return $"{SummaryHeader}\n\n{NoChangesMessage}";
        }

        var summaryBuilder = new StringBuilder();
        summaryBuilder.Append(SummaryHeader);
        summaryBuilder.AppendLine();
        summaryBuilder.AppendLine();

        AppendBreakingChangeWarning(summaryBuilder, commits);

        var changeTypesDescription = BuildChangeTypesDescription(commits);
        summaryBuilder.Append($"This branch includes {changeTypesDescription}.");

        return summaryBuilder.ToString();
    }

    private static void AppendBreakingChangeWarning(StringBuilder summaryBuilder, List<ConventionalCommit> commits)
    {
        if (commits.Any(commit => commit.IsBreakingChange))
        {
            summaryBuilder.Append(BreakingChangeWarning);
        }
    }

    private static string BuildChangeTypesDescription(List<ConventionalCommit> commits)
    {
        var commitTypes = commits
            .Select(commit => commit.Type)
            .Distinct()
            .ToHashSet();

        var changeDescriptions = CollectChangeDescriptions(commitTypes);

        return changeDescriptions.Count > 0
            ? string.Join(", ", changeDescriptions)
            : "changes";
    }

    private static List<string> CollectChangeDescriptions(HashSet<string> commitTypes)
    {
        var descriptions = new List<string>();
        var maintenanceTypes = CommitTypeMetadata.GetMaintenanceTypes().ToHashSet();
        var hasMaintenanceTypes = commitTypes.Any(type => maintenanceTypes.Contains(type));

        foreach (var type in commitTypes)
        {
            if (maintenanceTypes.Contains(type))
            {
                continue;
            }

            var metadata = CommitTypeMetadata.GetMetadata(type);
            descriptions.Add(metadata.Description);
        }

        if (hasMaintenanceTypes)
        {
            descriptions.Add("maintenance tasks");
        }

        return descriptions;
    }
}
