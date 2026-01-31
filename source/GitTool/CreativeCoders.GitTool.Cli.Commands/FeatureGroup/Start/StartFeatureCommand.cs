using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core.Abstractions;
using JetBrains.Annotations;
using IGitToolPullCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPullCommand;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Start;

[UsedImplicitly]
[CliCommand([FeatureCommandGroup.Name, "start"], Description = "Start a new feature branch")]
public class StartFeatureCommand(
    ISysConsole sysConsole,
    IRepositoryConfigurations repositoryConfigurations,
    IGitToolPullCommand pullCommand,
    IGitRepository gitRepository)
    : ICliCommand<StartFeatureOptions>
{
    private readonly IGitToolPullCommand _pullCommand = Ensure.NotNull(pullCommand);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    private readonly ISysConsole _sysConsole = Ensure.NotNull(sysConsole);

    private void PrintStartFeatureData(StartFeatureData data)
    {
        _sysConsole
            .WriteLine($"Create new feature branch '{data.FeatureBranch}' based on '{data.DefaultBranch}'")
            .WriteLine();
    }

    private void CheckIfFeatureBranchExists(StartFeatureData data)
    {
        var branch = gitRepository.Branches[data.FeatureBranch];

        if (branch != null)
        {
            _sysConsole
                .WriteLine();
        }

        //data.GitRepository.Remotes[data.FeatureBranch]
    }

    private void CreateAndCheckOutFeatureBranch(StartFeatureOptions options,
        RepositoryConfiguration repositoryConfiguration)
    {
        var featureBranchName = repositoryConfiguration.GetFeatureBranchName(options.FeatureName);

        _sysConsole.WriteLine($"Creating feature branch '{featureBranchName}' ...");

        var featureBranch = gitRepository.Branches.CreateBranch(featureBranchName);

        if (featureBranch == null)
        {
            _sysConsole.WriteLineError("Feature branch could not be created");

            return;
        }

        _sysConsole
            .WriteLine("Feature branch created")
            .WriteLine()
            .WriteLine("Checking out feature branch...");

        var checkedOutBranch = gitRepository.Branches.CheckOut(featureBranch.Name.Canonical);

        if (checkedOutBranch == null)
        {
            _sysConsole.WriteError("Feature Branch could not be checked out");

            return;
        }

        _sysConsole
            .WriteLine("Feature branch checked out")
            .WriteLine();
    }

    private async Task CheckOutAndUpdateBaseBranch(RepositoryConfiguration repositoryConfiguration)
    {
        var baseBranchName = repositoryConfiguration.GetDefaultBranchName(gitRepository.Info.MainBranch);

        if (string.IsNullOrEmpty(baseBranchName))
        {
            _sysConsole
                .WriteLineError($"There seems to be no base branch '{baseBranchName}'")
                .WriteLine()
                .WriteLine("Existing branches:");

            gitRepository.Branches.ForEach(branch =>
                _sysConsole.WriteLine($" {branch.Name.Friendly} -> {branch.TrackedBranch?.Name.Friendly}"));

            return;
        }

        _sysConsole.WriteLine($"Checkout base branch '{baseBranchName}'");

        gitRepository.Branches.CheckOut(baseBranchName);

        _sysConsole
            .WriteLine("Base branch checked out")
            .WriteLine()
            .WriteLine("Pulling from origin");

        await _pullCommand.ExecuteAsync(gitRepository).ConfigureAwait(false);

        gitRepository.Fetch("origin", new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    private StartFeatureData CreateData(StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        return new StartFeatureData(configuration.GetFeatureBranchName(options.FeatureName),
            configuration.GetDefaultBranchName(gitRepository.Info.MainBranch));
    }

    public async Task<CommandResult> ExecuteAsync(StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        var data = CreateData(options);

        PrintStartFeatureData(data);

        CheckIfFeatureBranchExists(data);

        await CheckOutAndUpdateBaseBranch(configuration).ConfigureAwait(false);

        CreateAndCheckOutFeatureBranch(options, configuration);

        if (options.PushAfterCreate)
        {
            _sysConsole.WriteLine("Pushing feature branch to remote...");

            gitRepository.Push(new GitPushOptions());

            _sysConsole
                .WriteLine("Feature branch pushed")
                .WriteLine();
        }

        return ReturnCodes.Success;
    }
}
