using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.RefSpecs;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.Common;

/// <summary>
/// Provides extension methods for converting between LibGit2Sharp types and their Git abstraction equivalents.
/// </summary>
public static class GitConvertExtensions
{
    /// <summary>
    /// Converts a LibGit2Sharp <see cref="RefSpecDirection"/> to a <see cref="GitRefSpecDirection"/>.
    /// </summary>
    /// <param name="refSpecDirection">The LibGit2Sharp ref spec direction.</param>
    /// <returns>The corresponding <see cref="GitRefSpecDirection"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized direction.</exception>
    public static GitRefSpecDirection ToGitRefSpecDirection(this RefSpecDirection refSpecDirection)
    {
        return refSpecDirection switch
        {
            RefSpecDirection.Fetch => GitRefSpecDirection.Fetch,
            RefSpecDirection.Push => GitRefSpecDirection.Push,
            _ => throw new ArgumentOutOfRangeException(nameof(refSpecDirection), refSpecDirection, null)
        };
    }

    /// <summary>
    /// Converts a LibGit2Sharp <see cref="MergeStatus"/> to a <see cref="GitMergeStatus"/>.
    /// </summary>
    /// <param name="mergeStatus">The LibGit2Sharp merge status.</param>
    /// <returns>The corresponding <see cref="GitMergeStatus"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized merge status.</exception>
    public static GitMergeStatus ToGitMergeStatus(this MergeStatus mergeStatus)
    {
        return mergeStatus switch
        {
            MergeStatus.UpToDate => GitMergeStatus.UpToDate,
            MergeStatus.FastForward => GitMergeStatus.FastForward,
            MergeStatus.NonFastForward => GitMergeStatus.NonFastForward,
            MergeStatus.Conflicts => GitMergeStatus.Conflicts,
            _ => throw new ArgumentOutOfRangeException(nameof(mergeStatus), mergeStatus, null)
        };
    }

    /// <summary>
    /// Converts a nullable <see cref="GitTagFetchMode"/> to a nullable LibGit2Sharp <see cref="TagFetchMode"/>.
    /// </summary>
    /// <param name="tagFetchMode">The Git tag fetch mode, or <see langword="null"/>.</param>
    /// <returns>The corresponding <see cref="TagFetchMode"/> value, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized tag fetch mode.</exception>
    public static TagFetchMode? ToTagFetchMode(this GitTagFetchMode? tagFetchMode)
    {
        if (tagFetchMode == null)
        {
            return null;
        }

        return tagFetchMode switch
        {
            GitTagFetchMode.All => TagFetchMode.All,
            GitTagFetchMode.Auto => TagFetchMode.Auto,
            GitTagFetchMode.FromConfigurationOrDefault => TagFetchMode.FromConfigurationOrDefault,
            GitTagFetchMode.None => TagFetchMode.None,
            _ => throw new ArgumentOutOfRangeException(nameof(tagFetchMode), tagFetchMode, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="GitFetchOptions"/> to a LibGit2Sharp <see cref="FetchOptions"/>.
    /// </summary>
    /// <param name="fetchOptions">The Git fetch options.</param>
    /// <param name="credentialsHandler">The credentials handler for authentication.</param>
    /// <returns>A new <see cref="FetchOptions"/> instance.</returns>
    public static FetchOptions ToFetchOptions(this GitFetchOptions fetchOptions,
        CredentialsHandler credentialsHandler)
    {
        return new FetchOptions
        {
            CredentialsProvider = credentialsHandler,
            Prune = fetchOptions.Prune,
            TagFetchMode = fetchOptions.TagFetchMode.ToTagFetchMode(),
            CustomHeaders = fetchOptions.CustomHeaders
        };
    }

    /// <summary>
    /// Converts a <see cref="GitFastForwardStrategy"/> to a LibGit2Sharp <see cref="FastForwardStrategy"/>.
    /// </summary>
    /// <param name="fastForwardStrategy">The Git fast-forward strategy.</param>
    /// <returns>The corresponding <see cref="FastForwardStrategy"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized fast-forward strategy.</exception>
    public static FastForwardStrategy ToFastForwardStrategy(this GitFastForwardStrategy fastForwardStrategy)
    {
        return fastForwardStrategy switch
        {
            GitFastForwardStrategy.Default => FastForwardStrategy.Default,
            GitFastForwardStrategy.FastForwardOnly => FastForwardStrategy.FastForwardOnly,
            GitFastForwardStrategy.NoFastForward => FastForwardStrategy.NoFastForward,
            _ => throw new ArgumentOutOfRangeException(nameof(fastForwardStrategy), fastForwardStrategy, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="GitCheckoutFileConflictStrategy"/> to a LibGit2Sharp <see cref="CheckoutFileConflictStrategy"/>.
    /// </summary>
    /// <param name="fileConflictStrategy">The Git checkout file conflict strategy.</param>
    /// <returns>The corresponding <see cref="CheckoutFileConflictStrategy"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized file conflict strategy.</exception>
    public static CheckoutFileConflictStrategy ToFileConflictStrategy(
        this GitCheckoutFileConflictStrategy fileConflictStrategy)
    {
        return fileConflictStrategy switch
        {
            GitCheckoutFileConflictStrategy.Diff3 => CheckoutFileConflictStrategy.Diff3,
            GitCheckoutFileConflictStrategy.Merge => CheckoutFileConflictStrategy.Merge,
            GitCheckoutFileConflictStrategy.Normal => CheckoutFileConflictStrategy.Normal,
            GitCheckoutFileConflictStrategy.Ours => CheckoutFileConflictStrategy.Ours,
            GitCheckoutFileConflictStrategy.Theirs => CheckoutFileConflictStrategy.Theirs,
            _ => throw new ArgumentOutOfRangeException(nameof(fileConflictStrategy), fileConflictStrategy, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="GitMergeFileFavor"/> to a LibGit2Sharp <see cref="MergeFileFavor"/>.
    /// </summary>
    /// <param name="mergeFileFavor">The Git merge file favor.</param>
    /// <returns>The corresponding <see cref="MergeFileFavor"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized merge file favor.</exception>
    public static MergeFileFavor ToMergeFileFavor(this GitMergeFileFavor mergeFileFavor)
    {
        return mergeFileFavor switch
        {
            GitMergeFileFavor.Normal => MergeFileFavor.Normal,
            GitMergeFileFavor.Ours => MergeFileFavor.Ours,
            GitMergeFileFavor.Theirs => MergeFileFavor.Theirs,
            GitMergeFileFavor.Union => MergeFileFavor.Union,
            _ => throw new ArgumentOutOfRangeException(nameof(mergeFileFavor), mergeFileFavor, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="GitMergeOptions"/> to a LibGit2Sharp <see cref="MergeOptions"/>.
    /// </summary>
    /// <param name="mergeOptions">The Git merge options.</param>
    /// <returns>A new <see cref="MergeOptions"/> instance.</returns>
    public static MergeOptions ToMergeOptions(this GitMergeOptions mergeOptions)
    {
        return new MergeOptions
        {
            CommitOnSuccess = mergeOptions.CommitOnSuccess,
            FailOnConflict = mergeOptions.FailOnConflict,
            FastForwardStrategy = mergeOptions.FastForwardStrategy.ToFastForwardStrategy(),
            FileConflictStrategy = mergeOptions.FileConflictStrategy.ToFileConflictStrategy(),
            FindRenames = mergeOptions.FindRenames,
            IgnoreWhitespaceChange = mergeOptions.IgnoreWhitespaceChange,
            MergeFileFavor = mergeOptions.MergeFileFavor.ToMergeFileFavor(),
            RenameThreshold = mergeOptions.RenameThreshold,
            SkipReuc = mergeOptions.SkipReuc,
            TargetLimit = mergeOptions.TargetLimit
        };
    }

    /// <summary>
    /// Converts a LibGit2Sharp <see cref="ChangeKind"/> to a <see cref="GitEntryChangeKind"/>.
    /// </summary>
    /// <param name="changeKind">The LibGit2Sharp change kind.</param>
    /// <returns>The corresponding <see cref="GitEntryChangeKind"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized change kind.</exception>
    public static GitEntryChangeKind ToGitEntryChangeKind(this ChangeKind changeKind)
    {
        return changeKind switch
        {
            ChangeKind.Unmodified => GitEntryChangeKind.Unmodified,
            ChangeKind.Added => GitEntryChangeKind.Added,
            ChangeKind.Deleted => GitEntryChangeKind.Deleted,
            ChangeKind.Modified => GitEntryChangeKind.Modified,
            ChangeKind.Renamed => GitEntryChangeKind.Renamed,
            ChangeKind.Copied => GitEntryChangeKind.Copied,
            ChangeKind.Ignored => GitEntryChangeKind.Ignored,
            ChangeKind.Untracked => GitEntryChangeKind.Untracked,
            ChangeKind.TypeChanged => GitEntryChangeKind.TypeChanged,
            ChangeKind.Unreadable => GitEntryChangeKind.Unreadable,
            ChangeKind.Conflicted => GitEntryChangeKind.Conflicted,
            _ => throw new ArgumentOutOfRangeException(nameof(changeKind), changeKind, null)
        };
    }

    /// <summary>
    /// Converts a LibGit2Sharp <see cref="Mode"/> to a <see cref="GitEntryMode"/>.
    /// </summary>
    /// <param name="mode">The LibGit2Sharp file mode.</param>
    /// <returns>The corresponding <see cref="GitEntryMode"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized file mode.</exception>
    public static GitEntryMode ToGitEntryMode(this Mode mode)
    {
        return mode switch
        {
            Mode.Directory => GitEntryMode.Directory,
            Mode.ExecutableFile => GitEntryMode.ExecutableFile,
            Mode.GitLink => GitEntryMode.GitLink,
            Mode.NonExecutableFile => GitEntryMode.NonExecutableFile,
            Mode.NonExecutableGroupWritableFile => GitEntryMode.NonExecutableGroupWritableFile,
            Mode.Nonexistent => GitEntryMode.Nonexistent,
            Mode.SymbolicLink => GitEntryMode.SymbolicLink,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}