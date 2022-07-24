using CreativeCoders.Git.Abstractions.Commits;
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

    private Func<string, GitCheckoutNotifyFlags, bool>? _checkoutNotify;

    private GitCheckoutNotifyFlags _checkoutNotifyFlags;

    public PullCommand(Repository repository, Func<CredentialsHandler> getCredentialsHandler,
        Func<Signature> getSignature)
    {
        _repository = Ensure.NotNull(repository, nameof(repository));
        _getCredentialsHandler = Ensure.NotNull(getCredentialsHandler, nameof(getCredentialsHandler));
        _getSignature = Ensure.NotNull(getSignature, nameof(getSignature));
    }

    public IPullCommand CheckoutNotify(Action<string, GitCheckoutNotifyFlags> notify)
    {
        return CheckoutNotify(notify, GitCheckoutNotifyFlags.All);
    }

    public IPullCommand CheckoutNotify(Action<string, GitCheckoutNotifyFlags> notify, GitCheckoutNotifyFlags notifyFlags)
    {
        Ensure.NotNull(notify, nameof(notify));

        _checkoutNotify = (path, checkoutNotifyFlag) =>
        {
            notify(path, checkoutNotifyFlag);

            return true;
        };
        _checkoutNotifyFlags = notifyFlags;

        return this;
    }

    public GitMergeResult Run()
    {
        var options = new PullOptions
        {
            FetchOptions = new FetchOptions
            {
                CredentialsProvider = _getCredentialsHandler(),
                //OnTransferProgress = OnTransferProgress
            },
            MergeOptions = new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.Default,
                CheckoutNotifyFlags = _checkoutNotifyFlags.ToCheckoutNotifyFlags(),
                OnCheckoutNotify = OnCheckoutNotify,
                //OnCheckoutProgress = OnCheckoutProgress
            }
        };

        var signature = _getSignature();

        var mergeResult = Commands.Pull(_repository, signature, options);

        return new GitMergeResult(mergeResult.Status.ToGitMergeStatus(), GitCommit.From(mergeResult.Commit));
    }

    private void OnCheckoutProgress(string path, int completedSteps, int totalSteps)
    {

    }

    private bool OnTransferProgress(TransferProgress progress)
    {
        //throw new NotImplementedException();

        return true;
    }

    private bool OnCheckoutNotify(string path, CheckoutNotifyFlags notifyFlags)
    {
        return _checkoutNotify == null || _checkoutNotify(path, notifyFlags.ToGitCheckoutNotifyFlags());
    }
}
