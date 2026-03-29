using System;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Certs;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.GitCommands;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.Abstractions.Tags;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Represents a Git repository and provides access to common Git operations.
/// </summary>
[PublicAPI]
public interface IGitRepository : IDisposable
{
    /// <summary>
    /// Pulls changes from the tracked remote branch into the current branch.
    /// </summary>
    /// <returns>The result of the merge operation performed during the pull.</returns>
    GitMergeResult Pull();

    /// <summary>
    /// Pushes local changes to the remote repository.
    /// </summary>
    /// <param name="gitPushOptions">The options controlling push behavior.</param>
    void Push(GitPushOptions gitPushOptions);

    /// <summary>
    /// Fetches changes from the specified remote.
    /// </summary>
    /// <param name="remoteName">The name of the remote to fetch from.</param>
    /// <param name="gitFetchOptions">The options controlling fetch behavior.</param>
    void Fetch(string remoteName, GitFetchOptions gitFetchOptions);

    /// <summary>
    /// Merges the source branch into the target branch.
    /// </summary>
    /// <param name="sourceBranchName">The name of the branch to merge from.</param>
    /// <param name="targetBranchName">The name of the branch to merge into.</param>
    /// <param name="mergeOptions">The options controlling merge behavior.</param>
    /// <returns>The result of the merge operation.</returns>
    GitMergeResult Merge(string sourceBranchName, string targetBranchName, GitMergeOptions mergeOptions);

    /// <summary>
    /// Determines whether the repository has uncommitted changes.
    /// </summary>
    /// <param name="includeUntracked"><see langword="true"/> to include untracked files; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the repository has uncommitted changes; otherwise, <see langword="false"/>.</returns>
    bool HasUncommittedChanges(bool includeUntracked);

    /// <summary>
    /// Gets the repository information.
    /// </summary>
    IGitRepositoryInfo Info { get; }

    /// <summary>
    /// Gets a value that indicates whether HEAD is detached.
    /// </summary>
    /// <value><see langword="true"/> if HEAD is detached; otherwise, <see langword="false"/>.</value>
    bool IsHeadDetached { get; }

    /// <summary>
    /// Gets the current HEAD branch.
    /// </summary>
    IGitBranch Head { get; }

    /// <summary>
    /// Gets the collection of tags in the repository.
    /// </summary>
    IGitTagCollection Tags { get; }

    /// <summary>
    /// Gets the collection of references in the repository.
    /// </summary>
    IGitReferenceCollection Refs { get; }

    /// <summary>
    /// Gets the collection of branches in the repository.
    /// </summary>
    IGitBranchCollection Branches { get; }

    /// <summary>
    /// Gets the commit log for the repository.
    /// </summary>
    IGitCommitLog Commits { get; }

    /// <summary>
    /// Gets the collection of remotes configured for the repository.
    /// </summary>
    IGitRemoteCollection Remotes { get; }

    /// <summary>
    /// Gets the differ for comparing tree changes in the repository.
    /// </summary>
    IGitDiffer Differ { get; }

    /// <summary>
    /// Gets the command factory for creating Git commands.
    /// </summary>
    IGitCommands Commands { get; }

    /// <summary>
    /// Gets or sets the handler invoked when a server certificate needs to be validated.
    /// </summary>
    HostCertificateCheckHandler? CertificateCheck { get; set; }
}
