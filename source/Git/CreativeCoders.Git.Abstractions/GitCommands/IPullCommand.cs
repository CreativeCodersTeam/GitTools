using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.Merges;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.GitCommands;

[PublicAPI]
public interface IPullCommand
{
    IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify);

    IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify);

    IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    IPullCommand OnCheckoutProgress(GitCheckoutProgressHandler progress);

    IPullCommand OnTransferProgress(Action<GitFetchTransferProgress> progress);

    GitMergeResult Run();
}
