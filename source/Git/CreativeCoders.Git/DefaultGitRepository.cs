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

        var context = new RepositoryContext(repo, libGitCaller, GetSignature, GetCredentialsHandler);

        Info = new GitRepositoryInfo(_repo);

        Branches = new GitBranchCollection(context);
        Tags = new GitTagCollection(context);
        Commits = new GitCommitLog(_repo.Commits);
        Refs = new GitReferenceCollection(_repo.Refs);
        Remotes = new GitRemoteCollection(_repo.Network.Remotes);
        Differ = new GitDiffer(_repo.Diff);
        Commands = new GitCommands.GitCommands(this, GetCredentialsHandler, GetSignature, libGitCaller);
    }

    public void Dispose()
    {
        _repo.Dispose();
    }

    public GitMergeResult Pull()
    {
        return Commands.CreatePullCommand().Run();
    }

    public void Push(GitPushOptions gitPushOptions)
    {
        Commands.CreatePushCommand()
            .CreateRemoteBranchIfNotExists(gitPushOptions.CreateRemoteBranchIfNotExists)
            .Run();
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

    public GitMergeResult Merge(string sourceBranchName, string targetBranchName, GitMergeOptions mergeOptions)
    {
        return _libGitCaller.Invoke(() =>
        {
            var sourceBranch = _repo.Branches[sourceBranchName];

            var targetBranch = Branches.CheckOut(targetBranchName);

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

    internal Repository LibGit2Repository => _repo;
}