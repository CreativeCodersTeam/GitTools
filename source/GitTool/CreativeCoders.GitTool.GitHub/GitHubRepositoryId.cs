using System;

namespace CreativeCoders.GitTool.GitHub;

public class GitHubRepositoryId
{
    public GitHubRepositoryId(Uri repositoryUrl)
    {
        var projectPath = repositoryUrl.AbsolutePath[1..];

        if (projectPath.EndsWith(".git", StringComparison.CurrentCultureIgnoreCase))
        {
            projectPath = projectPath[..^4];
        }

        var projectParts = projectPath.Split('/');

        if (projectParts.Length != 2)
        {
            throw new ArgumentException(
                $"Github project path could not be parsed. Expected parts = 2. Found = {projectParts.Length}");
        }

        Owner = projectParts[0];
        RepositoryName = projectParts[1];
    }

    public string Owner { get; set; }

    public string RepositoryName { get; set; }
}