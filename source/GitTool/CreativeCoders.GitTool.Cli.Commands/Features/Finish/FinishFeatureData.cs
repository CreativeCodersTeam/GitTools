using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Cli.Commands.Features.Finish;

public class FinishFeatureData
{
    public FinishFeatureData(IGitRepository repository, string featureBranch, string defaultBranch,
        string repositoryGitServiceProviderName, string? pullRequestTitle)
    {
        Repository = repository;
        FeatureBranch = featureBranch;
        DefaultBranch = defaultBranch;
        RepositoryGitServiceProviderName = repositoryGitServiceProviderName;
        PullRequestTitle = pullRequestTitle;
    }

    public string FeatureBranch { get; }

    public string DefaultBranch { get; }

    public string RepositoryGitServiceProviderName { get; }

    public string? PullRequestTitle { get; }

    public IGitRepository Repository { get; }
}
