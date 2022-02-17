using System;
using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Exceptions;
using GitLabApiClient;
using GitLabApiClient.Models.MergeRequests.Requests;

namespace CreativeCoders.GitTool.GitLab
{
    internal class DefaultGitLabServiceProvider : IGitServiceProvider
    {
        private readonly IGitLabClient _gitLabClient;

        public DefaultGitLabServiceProvider(IGitLabClient gitLabClient)
        {
            _gitLabClient = Ensure.NotNull(gitLabClient, nameof(gitLabClient));
        }

        public async Task<GitPullRequest> CreatePullRequestAsync(GitCreatePullRequest gitCreatePullRequest)
        {
            var newMergeRequest = new CreateMergeRequest(gitCreatePullRequest.SourceBranch,
                gitCreatePullRequest.TargetBranch,
                gitCreatePullRequest.Title);

            if (!string.IsNullOrWhiteSpace(gitCreatePullRequest.Description))
            {
                newMergeRequest.Description = gitCreatePullRequest.Description;
            }

            var projectId = GitLabProjectId.GetProjectId(gitCreatePullRequest.RepositoryUrl);

            var mergeRequest = await _gitLabClient.MergeRequests.CreateAsync(projectId, newMergeRequest);

            if (mergeRequest == null)
            {
                throw new CreatePullRequestFailedException();
            }

            return new GitPullRequest(mergeRequest.WebUrl);
        }

        public async Task<bool> PullRequestExists(Uri repositoryUrl, string sourceBranch, string targetBranch)
        {
            var projectId = GitLabProjectId.GetProjectId(repositoryUrl);

            var mergeRequests = await _gitLabClient.MergeRequests.GetAsync(projectId);

            return mergeRequests.Any(x =>
                x.SourceBranch == sourceBranch
                && x.TargetBranch == targetBranch);
        }
    }
}
