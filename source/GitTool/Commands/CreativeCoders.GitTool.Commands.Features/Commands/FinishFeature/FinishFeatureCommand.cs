using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature
{
    public class FinishFeatureCommand : IFinishFeatureCommand
    {
        private readonly IFinishFeatureSteps _finishFeatureSteps;

        private readonly IRepositoryConfigurations _repositoryConfigurations;

        private readonly IGitRepositoryFactory _gitRepositoryFactory;

        public FinishFeatureCommand(IGitRepositoryFactory gitRepositoryFactory,
            IRepositoryConfigurations repositoryConfigurations,
            IFinishFeatureSteps finishFeatureSteps)
        {
            _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations, nameof(repositoryConfigurations));
            _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
            _finishFeatureSteps = Ensure.NotNull(finishFeatureSteps, nameof(finishFeatureSteps));
        }

        public async Task<int> FinishFeatureAsync(FinishFeatureOptions options)
        {
            using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

            var data = CreateData(repository, options);

            _finishFeatureSteps.UpdateFeatureBranch(data);

            _finishFeatureSteps.MergeDefaultBranch(data);

            _finishFeatureSteps.PushFeatureBranch(data);

            // Ensure remote branch creation and use it for pull request

            await _finishFeatureSteps.CreatePullRequest(data).ConfigureAwait(false);

            repository.CheckOut(data.DefaultBranch);

            repository.DeleteLocalBranch(data.FeatureBranch);

            return ReturnCodes.Success;
        }

        private FinishFeatureData CreateData(IGitRepository gitRepository, FinishFeatureOptions options)
        {
            var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

            return new FinishFeatureData(gitRepository, configuration.GetFeatureBranchName(options.FeatureName),
                configuration.GetDefaultBranchName(gitRepository.Info.MainBranch), configuration.GitServiceProviderName,
                options.PullRequestTitle);
        }
    }
}
