using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.Merges;

namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IPullCommand
{
    IPullCommand CheckoutNotify(GitSimpleCheckoutNotifyHandler notify);

    IPullCommand CheckoutNotify(GitSimpleCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    IPullCommand CheckoutNotify(GitCheckoutNotifyHandler notify);

    IPullCommand CheckoutNotify(GitCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    IPullCommand CheckoutProgress(GitCheckoutProgressHandler progress);

    IPullCommand TransferProgress(Action<GitTransferProgress> progress);

    GitMergeResult Run();
}
