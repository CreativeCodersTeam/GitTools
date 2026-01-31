namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

public interface IFinishFeatureSteps
{
    void UpdateFeatureBranch(FinishFeatureData data);

    Task PushFeatureBranch(FinishFeatureData data);

    void MergeDefaultBranch(FinishFeatureData data);

    Task CreatePullRequest(FinishFeatureData data);
}
