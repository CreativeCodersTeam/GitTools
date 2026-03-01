using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CreativeCoders.GitTool.Base.PullRequestInfos;

public sealed partial class ConventionalCommit
{
    private const string BreakingChangePrefix = "BREAKING CHANGE:";
    private const char LineSeparator = '\n';
    private const string FooterSeparator = ": ";

    private static readonly Regex ConventionalCommitRegex = CreateConventionalCommitRegex();

    public ConventionalCommit(string type, string? scope, string description, string? body, string? footer,
        bool isBreakingChange)
    {
        Type = type;
        Scope = scope;
        Description = description;
        Body = body;
        Footer = footer;
        IsBreakingChange = isBreakingChange;
    }

    public string Type { get; }

    public string? Scope { get; }

    public string Description { get; }

    public string? Body { get; }

    public string? Footer { get; }

    public bool IsBreakingChange { get; }

    public static ConventionalCommit? Parse(string commitMessage)
    {
        if (string.IsNullOrWhiteSpace(commitMessage))
        {
            return null;
        }

        var lines = SplitMessageIntoLines(commitMessage);
        var header = ParseHeader(lines[0]);

        if (header == null)
        {
            return null;
        }

        var (body, footer, hasBreakingChangeInBody) = ParseBodyAndFooter(lines);
        var isBreakingChange = header.IsBreaking || hasBreakingChangeInBody;

        return new ConventionalCommit(
            header.Type,
            header.Scope,
            header.Description,
            body,
            footer,
            isBreakingChange);
    }

    private static string[] SplitMessageIntoLines(string commitMessage)
    {
        return commitMessage.Split(LineSeparator, StringSplitOptions.None);
    }

    private static CommitHeader? ParseHeader(string headerLine)
    {
        var trimmedHeader = headerLine.Trim();
        var match = ConventionalCommitRegex.Match(trimmedHeader);

        if (!match.Success)
        {
            return null;
        }

        return new CommitHeader(
            Type: match.Groups["type"].Value,
            Scope: match.Groups["scope"].Success ? match.Groups["scope"].Value : null,
            Description: match.Groups["description"].Value.Trim(),
            IsBreaking: match.Groups["breaking"].Success);
    }

    private static (string? Body, string? Footer, bool HasBreakingChange) ParseBodyAndFooter(string[] lines)
    {
        if (lines.Length <= 1)
        {
            return (null, null, false);
        }

        var bodyLines = new List<string>();
        var footerLines = new List<string>();
        var isInFooter = false;
        var hasBreakingChange = false;

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];

            if (IsFooterLine(line))
            {
                isInFooter = true;

                if (IsBreakingChangeLine(line))
                {
                    hasBreakingChange = true;
                }
            }

            AddLineToAppropriateSection(line, isInFooter, bodyLines, footerLines);
        }

        var body = JoinLines(bodyLines);
        var footer = JoinLines(footerLines);

        return (body, footer, hasBreakingChange);
    }

    private static bool IsFooterLine(string line)
    {
        return IsBreakingChangeLine(line) ||
               (line.Contains(FooterSeparator) && line.Length > 0 && char.IsUpper(line[0]));
    }

    private static bool IsBreakingChangeLine(string line)
    {
        return line.StartsWith(BreakingChangePrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static void AddLineToAppropriateSection(
        string line,
        bool isInFooter,
        List<string> bodyLines,
        List<string> footerLines)
    {
        if (isInFooter)
        {
            footerLines.Add(line);
        }
        else if (!string.IsNullOrWhiteSpace(line))
        {
            bodyLines.Add(line);
        }
    }

    private static string? JoinLines(List<string> lines)
    {
        return lines.Count > 0
            ? string.Join(LineSeparator, lines).Trim()
            : null;
    }

    [GeneratedRegex(@"^(?<type>\w+)(\((?<scope>[^\)]+)\))?(?<breaking>!)?: (?<description>.+)$")]
    private static partial Regex CreateConventionalCommitRegex();

    private sealed record CommitHeader(string Type, string? Scope, string Description, bool IsBreaking);
}
