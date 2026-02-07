using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Exceptions;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;
using IGitToolPushCommand = CreativeCoders.GitTool.Cli.Commands.Shared.IGitToolPushCommand;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

public class FinishFeatureSteps(
    IAnsiConsole ansiConsole,
    IGitServiceProviders gitServiceProviders,
    IGitToolPushCommand pushCommand)
    : IFinishFeatureSteps
{
    private readonly IGitServiceProviders _gitServiceProviders = Ensure.NotNull(gitServiceProviders);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitToolPushCommand _pushCommand = Ensure.NotNull(pushCommand);

    public void UpdateFeatureBranch(FinishFeatureData data)
    {
        _ansiConsole.WriteLine($"Checkout feature branch '{data.FeatureBranch}'");

        data.Repository.Branches.CheckOut(data.FeatureBranch);

        if (data.Repository.HasUncommittedChanges(true))
        {
            _ansiConsole.MarkupLines(
                "Feature branch has uncommitted changes. Please commit or revert and try again".ToErrorMarkup(),
                string.Empty);

            throw new FeatureFinishFailedException("Feature branch has uncommitted changes",
                ReturnCodes.BranchHasUncommittedChanges);
        }

        _ansiConsole.WriteLines("Feature branch checked out", string.Empty);

        if (data.Repository.Branches[data.FeatureBranch]?.TrackedBranch == null)
        {
            return;
        }

        _ansiConsole
            .WriteLine("Pull feature branch updates from remote");

        var mergeResult = data.Repository.Pull();

        _ansiConsole.WriteLine($"Pull merge result {mergeResult.MergeStatus}");

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _ansiConsole.MarkupLines("Pull feature branch updates from remote caused merge conflicts".ToErrorMarkup(),
                "Resolve conflicts and try again".ToErrorMarkup());

            throw new FeatureFinishFailedException("Merge feature branch from remote into local caused merge conflicts",
                ReturnCodes.MergeConflicts);
        }

        _ansiConsole.WriteLines("Feature branch updates pulled from remote", string.Empty);
    }

    public void MergeDefaultBranch(FinishFeatureData data)
    {
        _ansiConsole.WriteLine("Checkout Default branch");

        data.Repository.Branches.CheckOut(data.DefaultBranch);

        _ansiConsole.WriteLines("Default branch checked out", string.Empty, "Pull default branch updates from remote");

        var mergeResult = data.Repository.Pull();

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _ansiConsole.MarkupLines("Pull default branch updates from remote caused merge conflicts".ToErrorMarkup(),
                "Resolve conflicts and try again".ToErrorMarkup());

            throw new FeatureFinishFailedException("Merge default branch from remote into local caused merge conflicts",
                ReturnCodes.MergeConflicts);
        }

        _ansiConsole.WriteLines("Default branch updates pulled from remote", string.Empty);

        mergeResult = data.Repository.Merge(data.DefaultBranch, data.FeatureBranch, new GitMergeOptions());

        if (mergeResult.MergeStatus == GitMergeStatus.Conflicts)
        {
            _ansiConsole.MarkupLines("Merge default branch to feature branch caused merge conflicts".ToErrorMarkup(),
                "Resolve conflicts and try again".ToErrorMarkup());

            throw new FeatureFinishFailedException("Merge default branch to feature branch caused merge conflicts",
                ReturnCodes.MergeConflicts);
        }

        data.Repository.Branches.CheckOut(data.FeatureBranch);
    }

    public async Task PushFeatureBranch(FinishFeatureData data)
    {
        _ansiConsole
            .WriteLine("Push feature branch to remote");

        if (!data.Repository.Head.BranchIsPushedToRemote())
        {
            await _pushCommand.ExecuteAsync(data.Repository, true, false).ConfigureAwait(false);
        }

        _ansiConsole.WriteLines("Feature branch pushed to remote", string.Empty);
    }

    public async Task CreatePullRequest(FinishFeatureData data)
    {
        var provider =
            await _gitServiceProviders.GetServiceProviderAsync(data.Repository, data.RepositoryGitServiceProviderName);

        if (await provider.PullRequestExists(data.Repository.Info.RemoteUri, data.FeatureBranch,
                data.DefaultBranch))
        {
            _ansiConsole.WriteLine("Pull/merge request already exists");

            return;
        }

        var pullRequest = new GitCreatePullRequest(data.Repository.Info.RemoteUri,
            data.PullRequestTitle ?? $"Pull request for {data.FeatureBranch}", data.FeatureBranch,
            data.DefaultBranch);

        try
        {
            _ansiConsole.WriteLine("Create pull/merge request");

            await provider.CreatePullRequestAsync(pullRequest).ConfigureAwait(false);

            _ansiConsole.WriteLines(
                "Pull/merge request created",
                string.Empty
            );
        }
        catch (CreatePullRequestFailedException e)
        {
            _ansiConsole.MarkupLines(e.Message.Trim().ToErrorMarkup(), string.Empty);

            throw new FeatureFinishFailedException("Create pull request failed", ReturnCodes.GeneralError);
        }
    }
}
