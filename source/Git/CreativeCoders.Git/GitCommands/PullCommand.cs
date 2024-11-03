using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.Merges;
using CreativeCoders.Git.Commits;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Merges;

namespace CreativeCoders.Git.GitCommands;

internal class PullCommand : IPullCommand
{
    private readonly RepositoryContext _repositoryContext;

    private GitCheckoutNotifyHandler? _checkoutNotify;

    private GitCheckoutNotifyFlags _checkoutNotifyFlags;

    private GitCheckoutProgressHandler? _checkoutProgress;

    private Action<GitFetchTransferProgress>? _transferProgress;

    public PullCommand(RepositoryContext repositoryContext)
    {
        _repositoryContext = Ensure.NotNull(repositoryContext);
    }

    private void OnGitCheckoutProgress(string path, int completedSteps, int totalSteps)
    {
        _checkoutProgress?.Invoke(path, completedSteps, totalSteps);
    }

    private bool OnGitTransferProgress(TransferProgress progress)
    {
        _transferProgress?.Invoke(new GitFetchTransferProgress
        {
            IndexedObjects = progress.IndexedObjects,
            ReceivedBytes = progress.ReceivedBytes,
            ReceivedObjects = progress.ReceivedObjects,
            TotalObjects = progress.TotalObjects
        });

        return true;
    }

    private bool OnGitCheckoutNotify(string path, CheckoutNotifyFlags notifyFlags)
    {
        return _checkoutNotify == null || _checkoutNotify(path, notifyFlags.ToGitCheckoutNotifyFlags());
    }

    public IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify)
    {
        return OnCheckoutNotify(notify, GitCheckoutNotifyFlags.All);
    }

    public IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags)
    {
        return OnCheckoutNotify((path, checkoutNotifyFlag) =>
            {
                notify(path, checkoutNotifyFlag);

                return true;
            },
            notifyFlags);
    }

    public IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify)
    {
        return OnCheckoutNotify(notify, GitCheckoutNotifyFlags.All);
    }

    public IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags)
    {
        Ensure.NotNull(notify);

        _checkoutNotify = notify;

        _checkoutNotifyFlags = notifyFlags;

        return this;
    }

    public IPullCommand OnCheckoutProgress(GitCheckoutProgressHandler progress)
    {
        _checkoutProgress = Ensure.NotNull(progress);

        return this;
    }

    public IPullCommand OnTransferProgress(Action<GitFetchTransferProgress> progress)
    {
        _transferProgress = Ensure.NotNull(progress);

        return this;
    }

    public GitMergeResult Run()
    {
        var options = new PullOptions
        {
            FetchOptions = new FetchOptions
            {
                CredentialsProvider = _repositoryContext.GetCredentialsHandler(),
                OnTransferProgress = OnGitTransferProgress
            },
            MergeOptions = new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.Default,
                CheckoutNotifyFlags = _checkoutNotifyFlags.ToCheckoutNotifyFlags(),
                OnCheckoutNotify = OnGitCheckoutNotify,
                OnCheckoutProgress = OnGitCheckoutProgress
            }
        };

        var certCheckHandler = _repositoryContext.GetCertificateCheckHandler();
        if (certCheckHandler != null)
        {
            options.FetchOptions.CertificateCheck = certCheckHandler;
        }

        var signature = _repositoryContext.GetSignature();

        var mergeResult = _repositoryContext.LibGitCaller.Invoke(() =>
            Commands.Pull(_repositoryContext.LibGitRepository, signature, options));

        return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
    }
}
