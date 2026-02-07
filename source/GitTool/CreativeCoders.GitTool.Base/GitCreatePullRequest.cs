using System;
using CreativeCoders.Core;

namespace CreativeCoders.GitTool.Base;

public class GitCreatePullRequest(Uri projectUrl, string title, string sourceBranch, string targetBranch)
{
    public Uri RepositoryUrl { get; } = Ensure.NotNull(projectUrl);

    public string Title { get; } = Ensure.IsNotNullOrWhitespace(title);

    public string SourceBranch { get; } = Ensure.IsNotNullOrWhitespace(sourceBranch);

    public string TargetBranch { get; } = Ensure.IsNotNullOrWhitespace(targetBranch);

    public string? Description { get; set; }
}
