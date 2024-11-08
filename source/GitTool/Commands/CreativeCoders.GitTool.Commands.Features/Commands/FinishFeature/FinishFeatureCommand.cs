﻿using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;

[UsedImplicitly]
public class FinishFeatureCommand : IGitToolCommandWithOptions<FinishFeatureOptions>
{
    private readonly IFinishFeatureSteps _finishFeatureSteps;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    public FinishFeatureCommand(IRepositoryConfigurations repositoryConfigurations,
        IFinishFeatureSteps finishFeatureSteps)
    {
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);
        _finishFeatureSteps = Ensure.NotNull(finishFeatureSteps);
    }

    private FinishFeatureData CreateData(IGitRepository gitRepository, FinishFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        return new FinishFeatureData(gitRepository, configuration.GetFeatureBranchName(options.FeatureName),
            configuration.GetDefaultBranchName(gitRepository.Info.MainBranch), configuration.GitServiceProviderName,
            options.PullRequestTitle);
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, FinishFeatureOptions options)
    {
        var data = CreateData(gitRepository, options);

        _finishFeatureSteps.UpdateFeatureBranch(data);

        _finishFeatureSteps.MergeDefaultBranch(data);

        await _finishFeatureSteps.PushFeatureBranch(data).ConfigureAwait(false);

        // Ensure remote branch creation and use it for pull request

        await _finishFeatureSteps.CreatePullRequest(data).ConfigureAwait(false);

        gitRepository.Branches.CheckOut(data.DefaultBranch);

        gitRepository.Branches.DeleteLocalBranch(data.FeatureBranch);

        return ReturnCodes.Success;
    }
}
