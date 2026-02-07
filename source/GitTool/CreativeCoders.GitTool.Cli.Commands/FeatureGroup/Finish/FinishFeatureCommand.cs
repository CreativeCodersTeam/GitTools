using CreativeCoders.Cli.Core;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

[UsedImplicitly]
[CliCommand([FeatureCommandGroup.Name, "finish"], Description = "Finish a feature branch and create a pull request")]
public class FinishFeatureCommand(
    IAnsiConsole ansiConsole,
    IRepositoryConfigurations repositoryConfigurations,
    IFinishFeatureSteps finishFeatureSteps,
    IGitRepository gitRepository)
    : ICliCommand<FinishFeatureOptions>
{
    private readonly IFinishFeatureSteps _finishFeatureSteps = Ensure.NotNull(finishFeatureSteps);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    private FinishFeatureData CreateData(FinishFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(_gitRepository);

        return new FinishFeatureData(_gitRepository, GetFeatureBranchName(options, configuration),
            configuration.GetDefaultBranchName(_gitRepository.Info.MainBranch), configuration.GitServiceProviderName,
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
            _ansiConsole.MarkupLine($"[red]{e.Message}[/]");

            return e.ExitCode;
        }

        _gitRepository.Branches.CheckOut(data.DefaultBranch);

        _gitRepository.Branches.DeleteLocalBranch(data.FeatureBranch);

        return CommandResult.Success;
    }

    private string GetFeatureBranchName(FinishFeatureOptions options, RepositoryConfiguration configuration)
    {
        if (!string.IsNullOrEmpty(options.FeatureName))
        {
            return configuration.GetFeatureBranchName(options.FeatureName);
        }

        var currentBranchName = _gitRepository.Head.Name.Friendly;

        if (currentBranchName.StartsWith(configuration.FeatureBranchPrefix))
        {
            return currentBranchName;
        }

        throw new CliCommandAbortException("No feature name passed and current branch is not a feature branch.",
            ReturnCodes.NoFeatureBranchFound);
    }
}
