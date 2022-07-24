using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Merges;

namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IPullCommand
{
    IPullCommand CheckoutNotify(Action<string, GitCheckoutNotifyFlags> notify);

    IPullCommand CheckoutNotify(Action<string, GitCheckoutNotifyFlags> notify, GitCheckoutNotifyFlags notifyFlags);

    GitMergeResult Run();
}
