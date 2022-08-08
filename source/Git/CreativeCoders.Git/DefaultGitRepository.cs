using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.Git.Auth;
using CreativeCoders.Git.Branches;
using CreativeCoders.Git.Commits;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Diffs;
using CreativeCoders.Git.GitCommands;
using CreativeCoders.Git.References;
using CreativeCoders.Git.Remotes;
using CreativeCoders.Git.Tags;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

internal class DefaultGitRepository : IGitRepository
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly Repository _repo;

    private readonly ILibGitCaller _libGitCaller;

    public DefaultGitRepository(Repository repo, IGitCredentialProviders credentialProviders,
        ILibGitCaller libGitCaller)
    {
        _repo = Ensure.NotNull(repo, nameof(repo));
        _libGitCaller = Ensure.NotNull(libGitCaller, nameof(libGitCaller));

        _credentialProviders = Ensure.NotNull(credentialProviders, nameof(credentialProviders));

        Info = new GitRepositoryInfo(_repo);

        Branches = new GitBranchCollection(_repo.Branches);
        Tags = new GitTagCollection(_repo.Tags);
        Commits = new GitCommitLog(_repo.Commits);
        Refs = new GitReferenceCollection(_repo.Refs);
        Remotes = new GitRemoteCollection(_repo.Network.Remotes);
        Differ = new GitDiffer(_repo.Diff);
        Commands = new GitCommands.GitCommands(_repo, GetCredentialsHandler, GetSignature);
    }

    public void Dispose()
    {
        _repo.Dispose();
    }

    public IGitBranch? CheckOut(string branchName)
    {
        Ensure.Argument(branchName, nameof(branchName)).NotNullOrEmpty();

        var branch = Branches[branchName];

        if (branch == null)
        {
            throw new GitBranchNotExistsException(branchName);
        }

        var checkedOutBranch = _libGitCaller.Invoke(() => LibGit2Sharp.Commands.Checkout(_repo, _repo.Branches[branchName]));

        return GitBranch.From(checkedOutBranch);
    }

    public GitMergeResult Pull()
    {
        return new PullCommand(_repo, GetCredentialsHandler, GetSignature).Run();
    }

    public IGitBranch? CreateBranch(string branchName)
    {
        return GitBranch.From(_libGitCaller.Invoke(() => _repo.CreateBranch(branchName)));
    }

    public IGitTag CreateTag(string tagName)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repo.ApplyTag(tagName)));
    }

    public IGitTag CreateTag(string tagName, string objectish)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repo.ApplyTag(tagName, objectish)));
    }

    public IGitTag CreateTagWithMessage(string tagName, string message)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repo.ApplyTag(tagName, GetSignature(), message)));
    }

    public IGitTag CreateTagWithMessage(string tagName, string objectish, string message)
    {
        return new GitTag(
            _libGitCaller
                .Invoke(() => _repo.ApplyTag(tagName, objectish, GetSignature(), message)));
    }

    public void Push(GitPushOptions gitPushOptions)
    {
        _libGitCaller.Invoke(() =>
        {
            var pushBranch = _repo.Head;

            if (pushBranch.TrackedBranch == null)
            {
                if (!gitPushOptions.CreateRemoteBranchIfNotExists)
                {
                    throw new GitPushFailedException(
                        $"Branch '{pushBranch.FriendlyName}' has no tracking remote branch to push to");
                }

                var remoteOrigin = _repo.Network.Remotes[GitRemotes.Origin];

                _repo.Branches.Update(pushBranch,
                    b => b.Remote = remoteOrigin.Name,
                    b => b.UpstreamBranch = pushBranch.CanonicalName);
            }

            var pushOptions = new PushOptions
            {
                CredentialsProvider = GetCredentialsHandler()
            };

            _repo.Network.Push(pushBranch, pushOptions);
        });
    }

    public void PushTag(IGitTag tag)
    {
        var pushOptions = new PushOptions
        {
            CredentialsProvider = GetCredentialsHandler()
        };

        _libGitCaller.Invoke(() =>
            _repo.Network.Push(_repo.Network.Remotes[GitRemotes.Origin], tag.Name.Canonical, pushOptions));
    }

    public void PushAllTags()
    {
        Tags.ForEach(PushTag);
    }

    public void Fetch(string remoteName, GitFetchOptions gitFetchOptions)
    {
        var fetchOptions = gitFetchOptions.ToFetchOptions(GetCredentialsHandler());

        var remote = Remotes[remoteName];

        if (remote == null)
        {
            throw new GitRemoteNotFoundException(remoteName);
        }

        var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToArray();

        _libGitCaller.Invoke(() => LibGit2Sharp.Commands.Fetch(_repo, remote.Name, refSpecs, fetchOptions, null));
    }

    public void DeleteLocalBranch(string branchName)
    {
        var branch = _libGitCaller.Invoke(() => _repo.Branches[branchName]);

        if (branch.IsRemote)
        {
            return;
        }

        _libGitCaller.Invoke(() => _repo.Branches.Remove(branch));
    }

    public GitMergeResult Merge(string sourceBranchName, string targetBranchName, GitMergeOptions mergeOptions)
    {
        return _libGitCaller.Invoke(() =>
        {
            var sourceBranch = _repo.Branches[sourceBranchName];

            var targetBranch = CheckOut(targetBranchName);

            if (targetBranch == null)
            {
                throw new GitCheckoutFailedException(targetBranchName);
            }

            var mergeResult = _repo.Merge(sourceBranch, GetSignature(), mergeOptions.ToMergeOptions());

            return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
        });
    }

    public bool HasUncommittedChanges(bool includeUntracked)
    {
        using var treeChanges =
            _libGitCaller.Invoke(() => _repo.Diff.Compare<TreeChanges>(null, includeUntracked));

        return treeChanges.Count > 0;
    }

    private CredentialsHandler GetCredentialsHandler()
        => new GitCredentialsHandler(_credentialProviders).HandleCredentials;

    private Signature GetSignature()
    {
        return _libGitCaller.Invoke(() => _repo.Config.BuildSignature(DateTimeOffset.Now));
    }

    public IGitRepositoryInfo Info { get; }

    public bool IsHeadDetached => _libGitCaller.Invoke(() => _repo.Info.IsHeadDetached);

    public IGitBranch Head => GitBranch.From(_libGitCaller.Invoke(() => _repo.Head))!;

    public IGitTagCollection Tags { get; }

    public IGitReferenceCollection Refs { get; }

    public IGitBranchCollection Branches { get; }

    public IGitCommitLog Commits { get; }

    public IGitRemoteCollection Remotes { get; }

    public IGitDiffer Differ { get; }

    public IGitCommands Commands { get; }
}