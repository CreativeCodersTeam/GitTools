using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.GitCommands;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public class PushCommand : IPushCommand
{
    private readonly Repository _repository;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;

    private readonly ILibGitCaller _libGitCaller;

    private bool _createRemoteBranchIfNotExists;

    private IGitBranch? _branch;

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
                OnPushTransferProgress = OnPushTransferProgress,
                OnNegotiationCompletedBeforePush = OnNegotiationCompletedBeforePush,
                OnPackBuilderProgress = OnPackBuilderProgress,
                OnPushStatusError = OnPushStatusError
            };

            _repository.Network.Push(pushBranch, pushOptions);
        });
    }

    private void OnPushStatusError(PushStatusError pushStatusErrors)
    {
        Console.WriteLine($"Push error: {pushStatusErrors.Message}");
    }

    private bool OnPackBuilderProgress(PackBuilderStage stage, int current, int total)
    {
        Console.WriteLine($"Pack: {stage} stage  {current}/{total}");

        return true;
    }

    private bool OnNegotiationCompletedBeforePush(IEnumerable<PushUpdate> updates)
    {
        updates.ForEach(x => Console.WriteLine($"{x.SourceObjectId}:{x.SourceRefName} -> {x.DestinationObjectId}:{x.DestinationRefName}"));

        return true;
    }

    private bool OnPushTransferProgress(int current, int total, long bytes)
    {
        Console.WriteLine($"Transfer: {current}/{total} : {bytes} bytes");

        return true;
    }
}
