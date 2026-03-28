using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Branches;

/// <summary>
/// Represents the collection of branches in a Git repository.
/// </summary>
[PublicAPI]
public interface IGitBranchCollection : IEnumerable<IGitBranch>
{
    /// <summary>
    /// Checks out the branch with the specified name.
    /// </summary>
    /// <param name="branchName">The name of the branch to check out.</param>
    /// <returns>The checked-out branch, or <see langword="null"/> if the branch does not exist.</returns>
    IGitBranch? CheckOut(string branchName);

    /// <summary>
    /// Creates a new branch with the specified name from the current HEAD.
    /// </summary>
    /// <param name="branchName">The name of the branch to create.</param>
    /// <returns>The newly created branch, or <see langword="null"/> if creation failed.</returns>
    IGitBranch? CreateBranch(string branchName);

    /// <summary>
    /// Deletes the local branch with the specified name.
    /// </summary>
    /// <param name="branchName">The name of the local branch to delete.</param>
    void DeleteLocalBranch(string branchName);

    /// <summary>
    /// Gets the branch with the specified name.
    /// </summary>
    /// <param name="name">The name of the branch.</param>
    /// <returns>The matching branch, or <see langword="null"/> if no branch with the specified name exists.</returns>
    IGitBranch? this[string name] { get; }

    /// <summary>
    /// Finds a local branch by its friendly name.
    /// </summary>
    /// <param name="branchName">The friendly name of the local branch.</param>
    /// <returns>The matching local branch, or <see langword="null"/> if not found.</returns>
    IGitBranch? FindLocalBranchByFriendlyName(string branchName);

    /// <summary>
    /// Finds a remote-tracking branch by its friendly name.
    /// </summary>
    /// <param name="branchName">The friendly name of the remote branch.</param>
    /// <returns>The matching remote-tracking branch, or <see langword="null"/> if not found.</returns>
    IGitBranch? FindRemoteBranchByFriendlyName(string branchName);
}
