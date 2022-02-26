using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.GitTool.Base.Exceptions;
using CreativeCoders.SysConsole.App;
using CreativeCoders.SysConsole.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature;

public class FinishFeatureSteps : IFinishFeatureSteps
{
    private readonly IGitServiceProviders _gitServiceProviders;

    private readonly ISysConsole _sysConsole;

    private readonly ToolConfiguration _toolOptions;

    public FinishFeatureSteps(ISysConsole sysConsole, IGitServiceProviders gitServiceProviders,
        IOptions<ToolConfiguration> toolOptions)
    {
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
        _gitServiceProviders = Ensure.NotNull(gitServiceProviders, nameof(gitServiceProviders));
        _toolOptions = Ensure.NotNull(toolOptions, nameof(toolOptions)).Value;
    }

    public void UpdateFeatureBranch(FinishFeatureData data)
    {
        _sysConsole.WriteLine($"Checkout feature branch '{data.FeatureBranch}'");

        data.Repository.CheckOut(data.FeatureBranch);

        if (data.Repository.HasUncommittedChanges(true))
        {
            _sysConsole
                .WriteLineError("Feature branch has uncommitted changes. Please commit or revert and try again")
                .WriteLine();

            throw new ConsoleException(ReturnCodes.BranchHasUncommittedChanges,
                "Feature branch has uncommitted changes");
        }

        _sysConsole
            .WriteLine("Feature branch checked out")
            .WriteLine();

        if (data.Repository.Branches[data.FeatureBranch]?.TrackedBranch == null)
        {
            return;
        }

        _sysConsole
            .WriteLine("Pull feature branch updates from remote");

        var mergeResult = data.Repository.Pull();

        _sysConsole.WriteLine($"Pull merge result {mergeResult.MergeStatus}");

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _sysConsole
                .WriteLineError("Pull feature branch updates from remote caused merge conflicts")
                .WriteLineError("Resolve conflicts and try again");

            throw new ConsoleException(ReturnCodes.MergeConflicts,
                "Merge feature branch from remote into local caused merge conflicts");
        }

        _sysConsole
            .WriteLine("Feature branch updates pulled from remote")
            .WriteLine();
    }

    public void MergeDefaultBranch(FinishFeatureData data)
    {
        _sysConsole.WriteLine("Checkout Default branch");

        data.Repository.CheckOut(data.DefaultBranch);

        _sysConsole
            .WriteLine("Default branch checked out")
            .WriteLine()
            .WriteLine("Pull default branch updates from remote");

        var mergeResult = data.Repository.Pull();

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _sysConsole
                .WriteLineError("Pull default branch updates from remote caused merge conflicts")
                .WriteLineError("Resolve conflicts and try again");

            throw new ConsoleException(ReturnCodes.MergeConflicts,
                "Merge default branch from remote into local caused merge conflicts");
        }

        _sysConsole
            .WriteLine("Default branch updates pulled from remote")
            .WriteLine();

        mergeResult = data.Repository.Merge(data.DefaultBranch, data.FeatureBranch, new GitMergeOptions());

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _sysConsole
                .WriteLineError("Merge default branch to feature branch caused merge conflicts")
                .WriteLineError("Resolve conflicts and try again");

            throw new ConsoleException(ReturnCodes.MergeConflicts,
                "Merge default branch to feature branch caused merge conflicts");
        }

        data.Repository.CheckOut(data.FeatureBranch);
    }

    public void PushFeatureBranch(FinishFeatureData data)
    {
        _sysConsole
            .WriteLine("Push feature branch to remote");

        data.Repository.Push(new GitPushOptions());

        _sysConsole
            .WriteLine("Feature branch pushed to remote")
            .WriteLine();
    }

    public async Task CreatePullRequest(FinishFeatureData data)
    {
        var provider = await _gitServiceProviders.GetServiceProviderAsync(data.Repository, null);

        if (provider == null)
        {
            var providerName = string.IsNullOrEmpty(data.RepositoryGitServiceProviderName)
                ? _toolOptions.DefaultGitServiceProviderName
                : data.RepositoryGitServiceProviderName;

            provider = await _gitServiceProviders.GetServiceProviderAsync(data.Repository, providerName);
        }

        if (provider == null)
        {
            _sysConsole
                .WriteLineError("No git service provider found")
                .WriteLine();

            return;
        }

        if (await provider.PullRequestExists(data.Repository.Info.RemoteUri, data.FeatureBranch,
                data.DefaultBranch))
        {
            _sysConsole.WriteLine("Pull/merge request already exists");

            return;
        }

        var pullRequest = new GitCreatePullRequest(data.Repository.Info.RemoteUri,
            data.PullRequestTitle ?? $"Pull request for {data.FeatureBranch}", data.FeatureBranch,
            data.DefaultBranch);

        try
        {
            _sysConsole.WriteLine("Create pull/merge request");

            await provider.CreatePullRequestAsync(pullRequest).ConfigureAwait(false);

            _sysConsole
                .WriteLine("Pull/merge request created")
                .WriteLine();
        }
        catch (CreatePullRequestFailedException e)
        {
            _sysConsole
                .WriteLineError(e.Message.Trim())
                .WriteLine();

            throw new ConsoleException(ReturnCodes.GeneralError, "Create pull request failed");
        }
    }
}