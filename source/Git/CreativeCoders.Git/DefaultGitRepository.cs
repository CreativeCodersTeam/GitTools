using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Certs;
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

/// <summary>
/// Provides the default implementation of <see cref="IGitRepository"/> backed by LibGit2Sharp.
/// </summary>
internal sealed class DefaultGitRepository : IGitRepository
{
    private readonly IGitCredentialProviders _credentialProviders;

    private readonly ILibGitCaller _libGitCaller;

    private readonly Repository _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultGitRepository"/> class.
    /// </summary>
    /// <param name="repo">The underlying LibGit2Sharp repository.</param>
    /// <param name="credentialProviders">The credential providers for authentication.</param>
    /// <param name="libGitCaller">The caller used to invoke LibGit2Sharp operations with exception handling.</param>
    public DefaultGitRepository(Repository repo, IGitCredentialProviders credentialProviders,
        ILibGitCaller libGitCaller)
    {
        _repo = Ensure.NotNull(repo);
        _libGitCaller = Ensure.NotNull(libGitCaller);

        _credentialProviders = Ensure.NotNull(credentialProviders);

        var context = new RepositoryContext(this, repo, libGitCaller, GetSignature, GetCredentialsHandler,
            GetCertificateCheck);

        Info = new GitRepositoryInfo(_repo);

        Branches = new GitBranchCollection(context);
        Tags = new GitTagCollection(context);
        Commits = new GitCommitLog(_repo.Commits);
        Refs = new GitReferenceCollection(_repo.Refs);
        Remotes = new GitRemoteCollection(_repo.Network.Remotes);
        Differ = new GitDiffer(_repo.Diff);
        Commands = new GitCommands.GitCommands(context);
    }

    private CredentialsHandler GetCredentialsHandler()
        => new GitCredentialsHandler(_credentialProviders).HandleCredentials;

    private Signature GetSignature()
    {
        return _libGitCaller.Invoke(() => _repo.Config.BuildSignature(DateTimeOffset.Now));
    }

    private CertificateCheckHandler? GetCertificateCheck()
    {
        if (CertificateCheck == null)
        {
            return null;
        }

        return (certificate, valid, host) =>
        {
            SshCertificate? sshCertificate = null;

            if (certificate is CertificateSsh sshCert)
            {
                sshCertificate = new SshCertificate(sshCert.HashMD5, sshCert.HashSHA1, sshCert.HasMD5, sshCert.HasSHA1);
            }

            var x509Certificate = (certificate as CertificateX509)?.Certificate;

            var args = new CertificateCheckArgs(x509Certificate, sshCertificate, host);

            var handled = CertificateCheck(this, args);

            return handled
                ? args.IsValid
                : valid;
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _repo.Dispose();
    }

    /// <inheritdoc />
    public GitMergeResult Pull()
    {
        return Commands.CreatePullCommand().Run();
    }

    /// <inheritdoc />
    public void Push(GitPushOptions gitPushOptions)
    {
        Commands.CreatePushCommand()
            .CreateRemoteBranchIfNotExists(gitPushOptions.CreateRemoteBranchIfNotExists)
            .Run();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool HasUncommittedChanges(bool includeUntracked)
    {
        using var treeChanges =
            _libGitCaller.Invoke(() => _repo.Diff.Compare<TreeChanges>(null, includeUntracked));

        return treeChanges.Count > 0;
    }

    /// <summary>
    /// Gets the underlying LibGit2Sharp repository instance.
    /// </summary>
    internal Repository LibGit2Repository => _repo;

    /// <inheritdoc />
    public IGitRepositoryInfo Info { get; }

    /// <inheritdoc />
    public bool IsHeadDetached => _libGitCaller.Invoke(() => _repo.Info.IsHeadDetached);

    /// <inheritdoc />
    public IGitBranch Head => GitBranch.From(_libGitCaller.Invoke(() => _repo.Head))!;

    /// <inheritdoc />
    public IGitTagCollection Tags { get; }

    /// <inheritdoc />
    public IGitReferenceCollection Refs { get; }

    /// <inheritdoc />
    public IGitBranchCollection Branches { get; }

    /// <inheritdoc />
    public IGitCommitLog Commits { get; }

    /// <inheritdoc />
    public IGitRemoteCollection Remotes { get; }

    /// <inheritdoc />
    public IGitDiffer Differ { get; }

    /// <inheritdoc />
    public IGitCommands Commands { get; }

    /// <inheritdoc />
    public HostCertificateCheckHandler? CertificateCheck { get; set; }
}
