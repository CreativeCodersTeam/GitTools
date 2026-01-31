using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Configurations;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Features.Finish;

[UsedImplicitly]
[CliCommand([FeaturesCommandGroup.Name, "finish"], Description = "Finish a feature branch and create a pull request")]
public class FinishFeatureCommand(
    IAnsiConsole ansiConsole,
    IRepositoryConfigurations repositoryConfigurations,
    IFinishFeatureSteps finishFeatureSteps,
    IGitRepository gitRepository)
    : ICliCommand<FinishFeatureOptions>
{
    private readonly IFinishFeatureSteps _finishFeatureSteps = Ensure.NotNull(finishFeatureSteps);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    private FinishFeatureData CreateData(FinishFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        return new FinishFeatureData(gitRepository, configuration.GetFeatureBranchName(options.FeatureName),
            configuration.GetDefaultBranchName(gitRepository.Info.MainBranch), configuration.GitServiceProviderName,
            options.PullRequestTitle);
    }

    public async Task<CommandResult> ExecuteAsync(FinishFeatureOptions options)
    {
        var data = CreateData(options);

        try
        {
            _finishFeatureSteps.UpdateFeatureBranch(data);

            _finishFeatureSteps.MergeDefaultBranch(data);

            await _finishFeatureSteps.PushFeatureBranch(data).ConfigureAwait(false);

            // Ensure remote branch creation and use it for pull request

            await _finishFeatureSteps.CreatePullRequest(data).ConfigureAwait(false);
        }
        catch (FeatureFinishFailedException e)
        {
            ansiConsole.MarkupLine($"[red]{e.Message}[/]");

            return e.ExitCode;
        }

        gitRepository.Branches.CheckOut(data.DefaultBranch);

        gitRepository.Branches.DeleteLocalBranch(data.FeatureBranch);

        return CommandResult.Success;
    }
}
