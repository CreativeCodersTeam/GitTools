using System;
using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Base;

public interface IGitServiceProvider
{
    Task<GitPullRequest> CreatePullRequestAsync(GitCreatePullRequest gitCreatePullRequest);

    Task<bool> PullRequestExists(Uri repositoryUrl, string sourceBranch, string targetBranch);

    //Task<bool> MergePullRequest(int pullRequestNumber, bool squashMerge);
}