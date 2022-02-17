using System;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.RefSpecs;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.Common
{
    public static class GitConvertExtensions
    {
        public static GitRefSpecDirection ToGitRefSpecDirection(this RefSpecDirection refSpecDirection)
        {
            return refSpecDirection switch
            {
                RefSpecDirection.Fetch => GitRefSpecDirection.Fetch,
                RefSpecDirection.Push => GitRefSpecDirection.Push,
                _ => throw new ArgumentOutOfRangeException(nameof(refSpecDirection), refSpecDirection, null)
            };
        }

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
}
