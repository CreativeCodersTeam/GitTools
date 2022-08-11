using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.Pushes;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public class PushCommand : IPushCommand
{
    private readonly Repository _repository;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly ILibGitCaller _libGitCaller;

    private bool _createRemoteBranchIfNotExists;

    private IGitBranch? _branch;

    private Action<GitPushStatusError>? _pushStatusError;

    private Func<GitPackBuilderProgress, bool>? _packBuilderProgress;

    private Func<GitPushTransferProgress, bool>? _transferProgress;

    public PushCommand(Repository repository, Func<CredentialsHandler> getCredentialsHandler,
        ILibGitCaller libGitCaller)
    {
        _repository = Ensure.NotNull(repository, nameof(repository));
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _libGitCaller = Ensure.NotNull(libGitCaller, nameof(libGitCaller));
    }

    public IPushCommand CreateRemoteBranchIfNotExists()
        => CreateRemoteBranchIfNotExists(true);

    public IPushCommand CreateRemoteBranchIfNotExists(bool createRemoteBranchIfNotExists)
    {
        _createRemoteBranchIfNotExists = createRemoteBranchIfNotExists;

        return this;
    }

    public IPushCommand Branch(IGitBranch branch)
    {
        _branch = branch;

        return this;
    }

    public IPushCommand OnPushStatusError(Action<GitPushStatusError> pushStatusError)
    {
        _pushStatusError = pushStatusError;

        return this;
    }

    public IPushCommand OnPackBuilderProgress(Action<GitPackBuilderProgress> packBuilderProgress)
    {
        return OnPackBuilderProgress(x =>
        {
            packBuilderProgress(x);

            return true;
        });
    }

    public IPushCommand OnPackBuilderProgress(Func<GitPackBuilderProgress, bool> packBuilderProgress)
    {
        _packBuilderProgress = packBuilderProgress;

        return this;
    }

    public IPushCommand OnTransferProgress(Action<GitPushTransferProgress> transferProgress)
    {
        return OnTransferProgress(x =>
        {
            transferProgress(x);

            return true;
        });
    }

    public IPushCommand OnTransferProgress(Func<GitPushTransferProgress, bool> transferProgress)
    {
        _transferProgress = transferProgress;

        return this;
    }

    public void Run()
    {
        _libGitCaller.Invoke(() =>
        {
            var pushBranch = _branch != null
                ? _repository.Branches[_branch.Name.Canonical]
                : _repository.Head;

            if (pushBranch.TrackedBranch == null)
            {
                if (!_createRemoteBranchIfNotExists)
                {
                    throw new GitPushFailedException(
                        $"Branch '{pushBranch.FriendlyName}' has no tracking remote branch to push to");
                }

                var remoteOrigin = _repository.Network.Remotes[GitRemotes.Origin];

                _repository.Branches.Update(pushBranch,
                    b => b.Remote = remoteOrigin.Name,
                    b => b.UpstreamBranch = pushBranch.CanonicalName);
            }

            var pushOptions = new PushOptions
            {
                CredentialsProvider = _getCredentialsHandler(),
                OnPushTransferProgress = OnGitPushTransferProgress,
                OnNegotiationCompletedBeforePush = OnGitNegotiationCompletedBeforePush,
                OnPackBuilderProgress = OnGitPackBuilderProgress,
                OnPushStatusError = OnGitPushStatusError
            };

            _repository.Network.Push(pushBranch, pushOptions);
        });
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
        updates.ForEach(x => Console.WriteLine($"{x.SourceObjectId}:{x.SourceRefName} -> {x.DestinationObjectId}:{x.DestinationRefName}"));

        return true;
    }

    private bool OnGitPushTransferProgress(int current, int total, long bytes)
    {
        return _transferProgress?
                   .Invoke(new GitPushTransferProgress(current, total, bytes))
               ?? true;
    }
}
