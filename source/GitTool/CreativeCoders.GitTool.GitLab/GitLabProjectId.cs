using System;
using GitLabApiClient.Internal.Paths;

namespace CreativeCoders.GitTool.GitLab
{
    public static class GitLabProjectId
    {
        public static ProjectId GetProjectId(Uri repositoryUrl)
        {
            var repositoryPath = repositoryUrl.AbsolutePath[1..];

            if (repositoryPath.EndsWith(".git", StringComparison.CurrentCultureIgnoreCase))
            {
                repositoryPath = repositoryPath[..^4];
            }

            var projectParts = repositoryPath.Split('/');

            if (projectParts.Length < 2)
            {
                throw new ArgumentException(
                    $"Github project path could not be parsed. Expected parts = 2. Found = {projectParts.Length}");
            }

            ProjectId projectId = repositoryPath;

            return projectId;
        }
    }
}
