using System;
using CreativeCoders.Core;

namespace CreativeCoders.GitTool.Base
{
    public class GitCreatePullRequest
    {
        public GitCreatePullRequest(Uri projectUrl, string title, string sourceBranch, string targetBranch)
        {
            RepositoryUrl = Ensure.NotNull(projectUrl, nameof(projectUrl));
            Title = Ensure.IsNotNullOrWhitespace(title, nameof(title));
            SourceBranch = Ensure.IsNotNullOrWhitespace(sourceBranch, nameof(sourceBranch));
            TargetBranch = Ensure.IsNotNullOrWhitespace(targetBranch, nameof(targetBranch));
        }

        public Uri RepositoryUrl { get; }

        public string Title { get; }

        public string SourceBranch { get; }

        public string TargetBranch { get; }

        public string? Description { get; set; }
    }
}
