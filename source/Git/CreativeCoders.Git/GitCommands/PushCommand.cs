using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.Pushes;
using CreativeCoders.Git.Branches;
using CreativeCoders.Git.Objects;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

/// <summary>
/// Implements the push command that pushes local commits to a remote repository.
/// </summary>
internal class PushCommand : IPushCommand
{
    private readonly RepositoryContext _repositoryContext;

    private IGitBranch? _branch;

    private bool _confirm;

    private bool _createRemoteBranchIfNotExists;

    private Func<bool>? _doConfirm;

    private Func<IEnumerable<GitPushUpdate>, bool>? _negotiationCompletedBeforePush;

    private Func<GitPackBuilderProgress, bool>? _packBuilderProgress;

    private Action<GitPushStatusError>? _pushStatusError;

    private Func<GitPushTransferProgress, bool>? _transferProgress;

    private Action<IEnumerable<IGitCommit>>? _unPushedCommits;

    /// <summary>
    /// Initializes a new instance of the <see cref="PushCommand"/> class.
    /// </summary>
    /// <param name="repositoryContext">The repository context.</param>
    public PushCommand(RepositoryContext repositoryContext)
    {
        _repositoryContext = Ensure.NotNull(repositoryContext);
    }

    private void OnGitPushStatusError(PushStatusError pushStatusErrors)
    {
        _pushStatusError?
            .Invoke(new GitPushStatusError(pushStatusErrors.Reference, pushStatusErrors.Message));
    }

    private bool OnGitPackBuilderProgress(PackBuilderStage stage, int current, int total)
    {
        return _packBuilderProgress?
                   .Invoke(new GitPackBuilderProgress(stage.ToGitPackBuilderStage(), current, total))
               ?? true;
    }

    private bool OnGitNegotiationCompletedBeforePush(IEnumerable<PushUpdate> updates)
    {
        return _negotiationCompletedBeforePush?
                   .Invoke(
                       updates.Select(x =>
                               new GitPushUpdate(
                                   new GitObjectId(x.SourceObjectId), x.SourceRefName,
                                   new GitObjectId(x.DestinationObjectId), x.DestinationRefName))
                           .ToArray())
               ?? true;
    }

    private bool OnGitPushTransferProgress(int current, int total, long bytes)
    {
        return _transferProgress?
                   .Invoke(new GitPushTransferProgress(current, total, bytes))
               ?? true;
    }

    private void PushInternal()
    {
        var pushBranch = _branch != null
            ? _repositoryContext.LibGitRepository.Branches[_branch.Name.Canonical]
            : _repositoryContext.LibGitRepository.Head;

        if (pushBranch.TrackedBranch == null)
        {
            if (!_createRemoteBranchIfNotExists)
            {
                throw new GitPushFailedException(
                    $"Branch '{pushBranch.FriendlyName}' has no tracking remote branch to push to");
            }

            var remoteOrigin = _repositoryContext.LibGitRepository.Network.Remotes[GitRemotes.Origin];

            _repositoryContext.LibGitRepository.Branches.Update(pushBranch,
                b => b.Remote = remoteOrigin.Name,
                b => b.UpstreamBranch = pushBranch.CanonicalName);
        }

        var pushOptions = new PushOptions
        {
            CredentialsProvider = _repositoryContext.GetCredentialsHandler(),
            OnPushTransferProgress = OnGitPushTransferProgress,
            OnNegotiationCompletedBeforePush = OnGitNegotiationCompletedBeforePush,
            OnPackBuilderProgress = OnGitPackBuilderProgress,
            OnPushStatusError = OnGitPushStatusError
        };

        var unPushedCommits = new GitBranch(pushBranch)
            .UnPushedCommits()
            .ToArray();

        if (unPushedCommits.Length > 0)
        {
            _unPushedCommits?.Invoke(unPushedCommits);

            if (_confirm && _doConfirm != null)
            {
                var confirmed = _doConfirm.Invoke();

                if (!confirmed)
                {
                    return;
                }
            }
        }

        _repositoryContext.LibGitRepository.Network.Push(pushBranch, pushOptions);
    }

    /// <inheritdoc />
    public IPushCommand CreateRemoteBranchIfNotExists()
        => CreateRemoteBranchIfNotExists(true);

    /// <inheritdoc />
    public IPushCommand CreateRemoteBranchIfNotExists(bool createRemoteBranchIfNotExists)
    {
        _createRemoteBranchIfNotExists = createRemoteBranchIfNotExists;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand Branch(IGitBranch branch)
    {
        _branch = branch;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand Confirm()
    {
        return Confirm(true);
    }

    /// <inheritdoc />
    public IPushCommand Confirm(bool confirm)
    {
        _confirm = confirm;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnPushStatusError(Action<GitPushStatusError> pushStatusError)
    {
        _pushStatusError = pushStatusError;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnPackBuilderProgress(Action<GitPackBuilderProgress> packBuilderProgress)
    {
        return OnPackBuilderProgress(x =>
        {
            packBuilderProgress(x);

            return true;
        });
    }

    /// <inheritdoc />
    public IPushCommand OnPackBuilderProgress(Func<GitPackBuilderProgress, bool> packBuilderProgress)
    {
        _packBuilderProgress = packBuilderProgress;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnTransferProgress(Action<GitPushTransferProgress> transferProgress)
    {
        return OnTransferProgress(x =>
        {
            transferProgress(x);

            return true;
        });
    }

    /// <inheritdoc />
    public IPushCommand OnTransferProgress(Func<GitPushTransferProgress, bool> transferProgress)
    {
        _transferProgress = transferProgress;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnNegotiationCompletedBeforePush(
        Action<IEnumerable<GitPushUpdate>> negotiationCompletedBeforePush)
    {
        return OnNegotiationCompletedBeforePush(x =>
        {
            negotiationCompletedBeforePush(x);

            return true;
        });
    }

    /// <inheritdoc />
    public IPushCommand OnNegotiationCompletedBeforePush(
        Func<IEnumerable<GitPushUpdate>, bool> negotiationCompletedBeforePush)
    {
        _negotiationCompletedBeforePush = negotiationCompletedBeforePush;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnUnPushedCommits(Action<IEnumerable<IGitCommit>> unPushedCommits)
    {
        _unPushedCommits = unPushedCommits;

        return this;
    }

    /// <inheritdoc />
    public IPushCommand OnConfirm(Func<bool> doConfirm)
    {
        _doConfirm = doConfirm;

        return this;
    }

    /// <inheritdoc />
    public void Run()
    {
        _repositoryContext.LibGitCaller.Invoke(PushInternal);
    }
}
