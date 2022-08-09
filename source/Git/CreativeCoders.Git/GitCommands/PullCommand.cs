using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.Merges;
using CreativeCoders.Git.Commits;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Merges;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

internal class PullCommand : IPullCommand
{
    private readonly Repository _repository;

    private readonly Func<CredentialsHandler> _getCredentialsHandler;
    
    private readonly Func<Signature> _getSignature;

    private readonly ILibGitCaller _libGitCaller;

    private GitCheckoutNotifyHandler? _checkoutNotify;

    private GitCheckoutNotifyFlags _checkoutNotifyFlags;

    private GitCheckoutProgressHandler? _checkoutProgress;

    private Action<GitTransferProgress>? _transferProgress;

    public PullCommand(Repository repository, Func<CredentialsHandler> getCredentialsHandler,
        Func<Signature> getSignature, ILibGitCaller libGitCaller)
    {
        _repository = Ensure.NotNull(repository, nameof(repository));
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _getSignature = Ensure.NotNull(getSignature, nameof(getSignature));
        _libGitCaller = Ensure.NotNull(libGitCaller, nameof(libGitCaller));
    }

    public IPullCommand CheckoutNotify(GitSimpleCheckoutNotifyHandler notify)
    {
        return CheckoutNotify(notify, GitCheckoutNotifyFlags.All);
    }

    public IPullCommand CheckoutNotify(GitSimpleCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags)
    {
        return CheckoutNotify((path, checkoutNotifyFlag) =>
            {
                notify(path, checkoutNotifyFlag);

                return true;
            },
            notifyFlags);
    }

    public IPullCommand CheckoutNotify(GitCheckoutNotifyHandler notify)
    {
        return CheckoutNotify(notify, GitCheckoutNotifyFlags.All);
    }

    public IPullCommand CheckoutNotify(GitCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags)
    {
        Ensure.NotNull(notify, nameof(notify));

        _checkoutNotify = notify;

        _checkoutNotifyFlags = notifyFlags;

        return this;
    }

    public IPullCommand CheckoutProgress(GitCheckoutProgressHandler progress)
    {
        _checkoutProgress = Ensure.NotNull(progress, nameof(progress));

        return this;
    }

    public IPullCommand TransferProgress(Action<GitTransferProgress> progress)
    {
        _transferProgress = Ensure.NotNull(progress, nameof(progress));

        return this;
    }

    public GitMergeResult Run()
    {
        var options = new PullOptions
        {
            FetchOptions = new FetchOptions
            {
                CredentialsProvider = _getCredentialsHandler(),
                OnTransferProgress = OnTransferProgress
            },
            MergeOptions = new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.Default,
                CheckoutNotifyFlags = _checkoutNotifyFlags.ToCheckoutNotifyFlags(),
                OnCheckoutNotify = OnCheckoutNotify,
                OnCheckoutProgress = OnCheckoutProgress
            }
        };

        var signature = _getSignature();

        var mergeResult = _libGitCaller.Invoke(() => Commands.Pull(_repository, signature, options));

        return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
    }

    private void OnCheckoutProgress(string path, int completedSteps, int totalSteps)
    {
        _checkoutProgress?.Invoke(path, completedSteps, totalSteps);
    }

    private bool OnTransferProgress(TransferProgress progress)
    {
        _transferProgress?.Invoke(new GitTransferProgress
        {
            IndexedObjects = progress.IndexedObjects,
            ReceivedBytes = progress.ReceivedBytes,
            ReceivedObjects = progress.ReceivedObjects,
            TotalObjects = progress.TotalObjects
        });
        
        return true;
    }

    private bool OnCheckoutNotify(string path, CheckoutNotifyFlags notifyFlags)
    {
        return _checkoutNotify == null || _checkoutNotify(path, notifyFlags.ToGitCheckoutNotifyFlags());
    }
}
