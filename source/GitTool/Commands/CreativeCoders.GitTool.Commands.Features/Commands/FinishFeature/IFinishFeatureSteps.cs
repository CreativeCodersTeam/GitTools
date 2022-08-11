using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;

public interface IFinishFeatureSteps
{
    void UpdateFeatureBranch(FinishFeatureData data);

    Task PushFeatureBranch(FinishFeatureData data);

    void MergeDefaultBranch(FinishFeatureData data);

    Task CreatePullRequest(FinishFeatureData data);
}