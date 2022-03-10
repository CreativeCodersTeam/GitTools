using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;

public class StartFeatureData
{
    public StartFeatureData(IGitRepository gitRepository, string featureBranch, string defaultBranch)
    {
        GitRepository = gitRepository;
        FeatureBranch = featureBranch;
        DefaultBranch = defaultBranch;
    }

    public IGitRepository GitRepository { get; }

    public string FeatureBranch { get; }

    public string DefaultBranch { get; }
}