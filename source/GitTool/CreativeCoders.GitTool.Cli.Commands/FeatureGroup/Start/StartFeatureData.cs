using CreativeCoders.Core;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Start;

public class StartFeatureData(string featureBranch, string defaultBranch)
{
    public string FeatureBranch { get; } = Ensure.IsNotNullOrWhitespace(featureBranch);

    public string DefaultBranch { get; } = Ensure.IsNotNullOrWhitespace(defaultBranch);
}
