namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Start;

public class StartFeatureData(string featureBranch, string defaultBranch)
{
    public string FeatureBranch { get; } = featureBranch;

    public string DefaultBranch { get; } = defaultBranch;
}
