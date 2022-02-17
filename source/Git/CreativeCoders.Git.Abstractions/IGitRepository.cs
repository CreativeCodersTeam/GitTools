using System;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.Abstractions.Tags;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions
{
    [PublicAPI]
    public interface IGitRepository : IDisposable
    {
        IGitBranch? CheckOut(string branchName);

        GitMergeResult Pull();

        IGitBranch? CreateBranch(string branchName);

        void Push(GitPushOptions gitPushOptions);

        void Fetch(string remoteName, GitFetchOptions gitFetchOptions);

        void DeleteLocalBranch(string branchName);

        GitMergeResult Merge(string sourceBranchName, string targetBranchName, GitMergeOptions mergeOptions);

        bool HasUncommittedChanges(bool includeUntracked);

        IGitRepositoryInfo Info { get; }

        bool IsHeadDetached { get; }

        IGitBranch Head { get; }

        IGitTagCollection Tags { get; }

        IGitReferenceCollection Refs { get; }

        IGitBranchCollection Branches { get; }

        IGitCommitLog Commits { get; }

        IGitRemoteCollection Remotes { get; }

        IGitDiffer Differ { get; }
    }
}
