using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

public class FinishFeatureData(
    IGitRepository repository,
    string featureBranch,
    string defaultBranch,
    string repositoryGitServiceProviderName,
    string? pullRequestTitle)
{
    public string FeatureBranch { get; } = Ensure.IsNotNullOrWhitespace(featureBranch);

    public string DefaultBranch { get; } = Ensure.IsNotNullOrWhitespace(defaultBranch);

    public string RepositoryGitServiceProviderName { get; } =
        Ensure.IsNotNullOrWhitespace(repositoryGitServiceProviderName);

    public string? PullRequestTitle { get; } = pullRequestTitle;

    public IGitRepository Repository { get; } = Ensure.NotNull(repository);
}
