using CreativeCoders.Cli.Core;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;
using IGitToolPullCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPullCommand;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Start;

[UsedImplicitly]
[CliCommand([FeatureCommandGroup.Name, "start"], Description = "Start a new feature branch")]
public class StartFeatureCommand(
    IAnsiConsole ansiConsole,
    IRepositoryConfigurations repositoryConfigurations,
    IGitToolPullCommand pullCommand,
    IGitRepository gitRepository)
    : ICliCommand<StartFeatureOptions>
{
    private readonly IGitToolPullCommand _pullCommand = Ensure.NotNull(pullCommand);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    private void PrintStartFeatureData(StartFeatureData data)
    {
        _ansiConsole.WriteLines($"Create new feature branch '{data.FeatureBranch}' based on '{data.DefaultBranch}'",
            string.Empty);
    }

    private void CheckIfFeatureBranchExists(StartFeatureData data)
    {
        var branch = _gitRepository.Branches.FindLocalBranchByFriendlyName(data.FeatureBranch);

        if (branch != null)
        {
            throw new CliCommandAbortException($"Feature branch '{data.FeatureBranch}' already exists locally.",
                ReturnCodes.FeatureBranchAlreadyExistsLocal);
        }

        var remoteBranch = _gitRepository.Branches.FindRemoteBranchByFriendlyName(data.FeatureBranch);

        if (remoteBranch != null)
        {
            throw new CliCommandAbortException($"Feature branch '{data.FeatureBranch}' already exists on remote.",
                ReturnCodes.FeatureBranchAlreadyExistsRemote);
        }
    }

    private void CreateAndCheckOutFeatureBranch(StartFeatureOptions options,
        RepositoryConfiguration repositoryConfiguration)
    {
        var featureBranchName = repositoryConfiguration.GetFeatureBranchName(options.FeatureName);

        _ansiConsole.WriteLine($"Creating feature branch '{featureBranchName}' ...");

        var featureBranch = _gitRepository.Branches.CreateBranch(featureBranchName);

        if (featureBranch == null)
        {
            _ansiConsole.MarkupLine("Feature branch could not be created".ToErrorMarkup());

            return;
        }

        _ansiConsole.WriteLines("Feature branch created", string.Empty, "Checking out feature branch...");

        var checkedOutBranch = _gitRepository.Branches.CheckOut(featureBranch.Name.Canonical);

        if (checkedOutBranch == null)
        {
            _ansiConsole.Markup("Feature Branch could not be checked out".ToErrorMarkup());

            return;
        }

        _ansiConsole.MarkupLines("Feature branch checked out".ToSuccessMarkup(), string.Empty);
    }

    private async Task CheckOutAndUpdateBaseBranch(RepositoryConfiguration repositoryConfiguration)
    {
        var baseBranchName = repositoryConfiguration.GetDefaultBranchName(_gitRepository.Info.MainBranch);

        if (string.IsNullOrEmpty(baseBranchName))
        {
            _ansiConsole.MarkupLines($"There seems to be no base branch '{baseBranchName}'".ToErrorMarkup(),
                string.Empty, "Existing branches:");

            _gitRepository.Branches.ForEach(branch =>
                _ansiConsole.WriteLine($" {branch.Name.Friendly} -> {branch.TrackedBranch?.Name.Friendly}"));

            return;
        }

        _ansiConsole.WriteLine($"Checkout base branch '{baseBranchName}'");

        _gitRepository.Branches.CheckOut(baseBranchName);

        _ansiConsole.WriteLines("Base branch checked out", string.Empty, "Pulling from origin");

        await _pullCommand.ExecuteAsync(_gitRepository).ConfigureAwait(false);

        _gitRepository.Fetch("origin", new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    private StartFeatureData CreateData(StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(_gitRepository);

        return new StartFeatureData(configuration.GetFeatureBranchName(options.FeatureName),
            configuration.GetDefaultBranchName(_gitRepository.Info.MainBranch));
    }

    public async Task<CommandResult> ExecuteAsync(StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(_gitRepository);

        var data = CreateData(options);

        PrintStartFeatureData(data);

        CheckIfFeatureBranchExists(data);

        await CheckOutAndUpdateBaseBranch(configuration).ConfigureAwait(false);

        CreateAndCheckOutFeatureBranch(options, configuration);

        if (!options.PushAfterCreate)
        {
            return ReturnCodes.Success;
        }

        _ansiConsole.WriteLine("Pushing feature branch to remote...");

        _gitRepository.Push(new GitPushOptions());

        _ansiConsole.WriteLines("Feature branch pushed", string.Empty);

        return ReturnCodes.Success;
    }
}
