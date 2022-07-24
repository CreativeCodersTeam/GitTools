using CreativeCoders.Git.Abstractions.Merges;

namespace CreativeCoders.Git.Merges;

public static class GitCheckoutNotifyFlagsExtensions
{
    public static CheckoutNotifyFlags ToCheckoutNotifyFlags(this GitCheckoutNotifyFlags checkoutNotifyFlags)
    {
        return new CheckoutNotifyFlags()
            .AddFlagIfSet(CheckoutNotifyFlags.Conflict, checkoutNotifyFlags, GitCheckoutNotifyFlags.Conflict)
            .AddFlagIfSet(CheckoutNotifyFlags.Dirty, checkoutNotifyFlags, GitCheckoutNotifyFlags.Dirty)
            .AddFlagIfSet(CheckoutNotifyFlags.Updated, checkoutNotifyFlags, GitCheckoutNotifyFlags.Updated)
            .AddFlagIfSet(CheckoutNotifyFlags.Ignored, checkoutNotifyFlags, GitCheckoutNotifyFlags.Ignored)
            .AddFlagIfSet(CheckoutNotifyFlags.Untracked, checkoutNotifyFlags, GitCheckoutNotifyFlags.Untracked);
    }

    public static GitCheckoutNotifyFlags ToGitCheckoutNotifyFlags(this CheckoutNotifyFlags checkoutNotifyFlags)
    {
        return new GitCheckoutNotifyFlags()
            .AddFlagIfSet(GitCheckoutNotifyFlags.Conflict, checkoutNotifyFlags, CheckoutNotifyFlags.Conflict)
            .AddFlagIfSet(GitCheckoutNotifyFlags.Dirty, checkoutNotifyFlags, CheckoutNotifyFlags.Dirty)
            .AddFlagIfSet(GitCheckoutNotifyFlags.Updated, checkoutNotifyFlags, CheckoutNotifyFlags.Updated)
            .AddFlagIfSet(GitCheckoutNotifyFlags.Ignored, checkoutNotifyFlags, CheckoutNotifyFlags.Ignored)
            .AddFlagIfSet(GitCheckoutNotifyFlags.Untracked, checkoutNotifyFlags, CheckoutNotifyFlags.Untracked);
    }

    private static TResult AddFlagIfSet<TResult, T>(this TResult flags, TResult addFlag, T checkFlags, T checkFlag)
        where TResult : Enum
        where T : Enum
    {
        if (!checkFlags.HasFlag(checkFlag))
        {
            return flags;
        }

        var intFlags = (int)(object)flags | (int)(object)addFlag;

        flags = (TResult) (object) intFlags;
        return flags;
    }
}
