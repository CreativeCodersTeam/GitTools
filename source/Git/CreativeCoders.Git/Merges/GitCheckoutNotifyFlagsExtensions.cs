using CreativeCoders.Git.Abstractions.Merges;

namespace CreativeCoders.Git.Merges;

/// <summary>
/// Provides extension methods for converting between <see cref="GitCheckoutNotifyFlags"/> and LibGit2Sharp <see cref="CheckoutNotifyFlags"/>.
/// </summary>
public static class GitCheckoutNotifyFlagsExtensions
{
    /// <summary>
    /// Converts a <see cref="GitCheckoutNotifyFlags"/> to a LibGit2Sharp <see cref="CheckoutNotifyFlags"/>.
    /// </summary>
    /// <param name="checkoutNotifyFlags">The Git checkout notify flags.</param>
    /// <returns>The corresponding <see cref="CheckoutNotifyFlags"/> value.</returns>
    public static CheckoutNotifyFlags ToCheckoutNotifyFlags(this GitCheckoutNotifyFlags checkoutNotifyFlags)
    {
        return new CheckoutNotifyFlags()
            .AddFlagIfSet(CheckoutNotifyFlags.Conflict, checkoutNotifyFlags, GitCheckoutNotifyFlags.Conflict)
            .AddFlagIfSet(CheckoutNotifyFlags.Dirty, checkoutNotifyFlags, GitCheckoutNotifyFlags.Dirty)
            .AddFlagIfSet(CheckoutNotifyFlags.Updated, checkoutNotifyFlags, GitCheckoutNotifyFlags.Updated)
            .AddFlagIfSet(CheckoutNotifyFlags.Ignored, checkoutNotifyFlags, GitCheckoutNotifyFlags.Ignored)
            .AddFlagIfSet(CheckoutNotifyFlags.Untracked, checkoutNotifyFlags, GitCheckoutNotifyFlags.Untracked);
    }

    /// <summary>
    /// Converts a LibGit2Sharp <see cref="CheckoutNotifyFlags"/> to a <see cref="GitCheckoutNotifyFlags"/>.
    /// </summary>
    /// <param name="checkoutNotifyFlags">The LibGit2Sharp checkout notify flags.</param>
    /// <returns>The corresponding <see cref="GitCheckoutNotifyFlags"/> value.</returns>
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
