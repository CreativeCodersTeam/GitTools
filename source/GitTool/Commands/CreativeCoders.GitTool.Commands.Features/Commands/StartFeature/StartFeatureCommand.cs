using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Commands.Shared;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Commands.Features.Commands.StartFeature;

public class StartFeatureCommand : IGitToolCommandWithOptions<StartFeatureOptions>
{
    private readonly IRepositoryConfigurations _repositoryConfigurations;

    private readonly ISysConsole _sysConsole;

    private readonly IGitToolPullCommand _pullCommand;

    public StartFeatureCommand(ISysConsole sysConsole,
        IRepositoryConfigurations repositoryConfigurations,
        IGitToolPullCommand pullCommand)
    {
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations, nameof(repositoryConfigurations));

        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));

        _pullCommand = Ensure.NotNull(pullCommand, nameof(pullCommand));
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        var data = CreateData(gitRepository, options);

        PrintStartFeatureData(data);

        CheckIfFeatureBranchExists(data);

        await CheckOutAndUpdateBaseBranch(gitRepository, configuration).ConfigureAwait(false);

        CreateAndCheckOutFeatureBranch(gitRepository, options, configuration);

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

    private void PrintStartFeatureData(StartFeatureData data)
    {
        _sysConsole
            .WriteLine($"Create new feature branch '{data.FeatureBranch}' based on '{data.DefaultBranch}'")
            .WriteLine();
    }

    private void CheckIfFeatureBranchExists(StartFeatureData data)
    {
        var branch = data.GitRepository.Branches[data.FeatureBranch];

        if (branch != null)
        {
            _sysConsole
                .WriteLine();
        }

        //data.GitRepository.Remotes[data.FeatureBranch]
    }

    private void CreateAndCheckOutFeatureBranch(IGitRepository repository,
        StartFeatureOptions options, RepositoryConfiguration repositoryConfiguration)
    {
        var featureBranchName = repositoryConfiguration.GetFeatureBranchName(options.FeatureName);

        _sysConsole.WriteLine($"Creating feature branch '{featureBranchName}' ...");

        var featureBranch = repository.CreateBranch(featureBranchName);

        if (featureBranch == null)
        {
            _sysConsole.WriteLineError("Feature branch could not be created");

            return;
        }

        _sysConsole
            .WriteLine("Feature branch created")
            .WriteLine()
            .WriteLine("Checking out feature branch...");

        var checkedOutBranch = repository.CheckOut(featureBranch.Name.Canonical);

        if (checkedOutBranch == null)
        {
            _sysConsole.WriteError("Feature Branch could not be checked out");

            return;
        }

        _sysConsole
            .WriteLine("Feature branch checked out")
            .WriteLine();
    }

    private async Task CheckOutAndUpdateBaseBranch(IGitRepository repository,
        RepositoryConfiguration repositoryConfiguration)
    {
        var baseBranchName = repositoryConfiguration.GetDefaultBranchName(repository.Info.MainBranch);

        if (string.IsNullOrEmpty(baseBranchName))
        {
            _sysConsole
                .WriteLineError($"There seems to be no base branch '{baseBranchName}'")
                .WriteLine()
                .WriteLine("Existing branches:");

            repository.Branches.ForEach(branch =>
                _sysConsole.WriteLine($" {branch.Name.Friendly} -> {branch.TrackedBranch?.Name.Friendly}"));

            return;
        }

        _sysConsole.WriteLine($"Checkout base branch '{baseBranchName}'");

        repository.CheckOut(baseBranchName);

        _sysConsole
            .WriteLine("Base branch checked out")
            .WriteLine()
            .WriteLine("Pulling from origin");

        await _pullCommand.ExecuteAsync(repository).ConfigureAwait(false);

        repository.Fetch("origin", new GitFetchOptions { TagFetchMode = GitTagFetchMode.All });
    }

    private StartFeatureData CreateData(IGitRepository gitRepository, StartFeatureOptions options)
    {
        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        return new StartFeatureData(gitRepository, configuration.GetFeatureBranchName(options.FeatureName),
            configuration.GetDefaultBranchName(gitRepository.Info.MainBranch));
    }
}