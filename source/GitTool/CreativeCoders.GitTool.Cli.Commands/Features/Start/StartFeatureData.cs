using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Cli.Commands.Features.Start;

public class StartFeatureData(string featureBranch, string defaultBranch)
{
    public string FeatureBranch { get; } = featureBranch;

    public string DefaultBranch { get; } = defaultBranch;
}
