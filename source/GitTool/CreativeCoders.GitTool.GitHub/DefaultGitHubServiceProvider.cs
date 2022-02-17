using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Exceptions;
using Octokit;

namespace CreativeCoders.GitTool.GitHub
{
    internal class DefaultGitHubServiceProvider : IGitServiceProvider
    {
        private readonly IGitHubClient _gitHubClient;

        public DefaultGitHubServiceProvider(IGitHubClient gitHubClient)
        {
            _gitHubClient = Ensure.Argument(gitHubClient, nameof(gitHubClient)).NotNull().Value;
        }

        public async Task<GitPullRequest> CreatePullRequestAsync(GitCreatePullRequest gitCreatePullRequest)
        {
            var newPullRequest = new NewPullRequest(gitCreatePullRequest.Title, gitCreatePullRequest.SourceBranch,
                gitCreatePullRequest.TargetBranch);

            if (!string.IsNullOrWhiteSpace(gitCreatePullRequest.Description))
            {
                newPullRequest.Body = gitCreatePullRequest.Description;
            }

            var repositoryId = new GitHubRepositoryId(gitCreatePullRequest.RepositoryUrl);

            try
            {
                var pullRequest = await _gitHubClient.PullRequest.Create(repositoryId.Owner,
                    repositoryId.RepositoryName, newPullRequest);

                return new GitPullRequest(pullRequest.Url);
            }
            catch (ApiException e)
            {
                var sb = new StringBuilder()
                    .AppendLine("Create pull/merge request failed.")
                    .AppendLine(e.ApiError.Message);

                e.ApiError.Errors.ForEach(x =>
                {
                    var fieldCode = $"{x.Field}.{x.Code}".Trim().Trim('.');

                    sb.AppendLine($"{x.Resource}({fieldCode}): {x.Message}");
                });

                throw new CreatePullRequestFailedException(sb.ToString(), e);
            }
        }

        public async Task<bool> PullRequestExists(Uri repositoryUrl, string sourceBranch, string targetBranch)
        {
            var repositoryId = new GitHubRepositoryId(repositoryUrl);

            var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(repositoryId.Owner,
                repositoryId.RepositoryName);

            return pullRequests.Any(x =>
                x.Head.Ref == sourceBranch
                && x.Base.Ref == targetBranch);
        }
    }
}
