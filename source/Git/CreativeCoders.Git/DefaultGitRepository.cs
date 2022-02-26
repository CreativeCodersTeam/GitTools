using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.Git.Auth;
using CreativeCoders.Git.Branches;
using CreativeCoders.Git.Commits;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Diffs;
using CreativeCoders.Git.References;
using CreativeCoders.Git.Remotes;
using CreativeCoders.Git.Tags;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git;

internal class DefaultGitRepository : IGitRepository
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly Repository _repo;

    public DefaultGitRepository(Repository repo, IGitCredentialProviders credentialProviders)
    {
        _repo = Ensure.NotNull(repo, nameof(repo));

        _credentialProviders = Ensure.NotNull(credentialProviders, nameof(credentialProviders));

        Info = new GitRepositoryInfo(_repo);

        Branches = new GitBranchCollection(_repo.Branches);
        Tags = new GitTagCollection(_repo.Tags);
        Commits = new GitCommitLog(_repo.Commits);
        Refs = new GitReferenceCollection(_repo.Refs);
        Remotes = new GitRemoteCollection(_repo.Network.Remotes);
        Differ = new GitDiffer(_repo.Diff);
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

        var checkedOutBranch = Commands.Checkout(_repo, _repo.Branches[branchName]);

        return GitBranch.From(checkedOutBranch);
    }

    public GitMergeResult Pull()
    {
        var options = new PullOptions
        {
            FetchOptions = new FetchOptions
            {
                CredentialsProvider = GetCredentialsHandler()
            },
            MergeOptions = new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.Default
            }
        };

        var signature = GetSignature();

        var mergeResult = Commands.Pull(_repo, signature, options);

        return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
    }

    public IGitBranch? CreateBranch(string branchName)
    {
        return GitBranch.From(_repo.CreateBranch(branchName));
    }

    public void Push(GitPushOptions gitPushOptions)
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

        Commands.Fetch(_repo, remote.Name, refSpecs, fetchOptions, null);
    }

    public void DeleteLocalBranch(string branchName)
    {
        var branch = _repo.Branches[branchName];

        if (branch.IsRemote)
        {
            return;
        }

        _repo.Branches.Remove(branch);
    }

    public GitMergeResult Merge(string sourceBranchName, string targetBranchName, GitMergeOptions mergeOptions)
    {
        var sourceBranch = _repo.Branches[sourceBranchName];

        var targetBranch = CheckOut(targetBranchName);

        if (targetBranch == null)
        {
            throw new GitCheckoutFailedException(targetBranchName);
        }

        var mergeResult = _repo.Merge(sourceBranch, GetSignature(), mergeOptions.ToMergeOptions());

        return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
    }

    public bool HasUncommittedChanges(bool includeUntracked)
    {
        using var treeChanges = _repo.Diff.Compare<TreeChanges>(null, includeUntracked);

        return treeChanges.Count > 0;
    }

    private CredentialsHandler GetCredentialsHandler()
        => new GitCredentialsHandler(_credentialProviders).HandleCredentials;

    private Signature GetSignature()
    {
        return _repo.Config.BuildSignature(DateTimeOffset.Now);
    }

    public IGitRepositoryInfo Info { get; }

    public bool IsHeadDetached => _repo.Info.IsHeadDetached;

    public IGitBranch Head => GitBranch.From(_repo.Head)!;

    public IGitTagCollection Tags { get; }

    public IGitReferenceCollection Refs { get; }

    public IGitBranchCollection Branches { get; }

    public IGitCommitLog Commits { get; }

    public IGitRemoteCollection Remotes { get; }

    public IGitDiffer Differ { get; }
}