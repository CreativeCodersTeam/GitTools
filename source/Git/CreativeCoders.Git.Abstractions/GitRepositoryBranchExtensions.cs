using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Exceptions;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

/// <summary>
/// Provides extension methods for branch operations on <see cref="IGitRepository"/> and <see cref="IGitBranch"/>.
/// </summary>
[PublicAPI]
public static class GitRepositoryBranchExtensions
{
    /// <summary>
    /// Creates a new branch from the specified source branch.
    /// </summary>
    /// <param name="gitRepository">The repository in which to create the branch.</param>
    /// <param name="sourceBranchName">The name of the source branch to branch from.</param>
    /// <param name="newBranchName">The name of the new branch to create.</param>
    /// <param name="updateSourceBranchBefore"><see langword="true"/> to fetch tags and pull the source branch before creating; otherwise, <see langword="false"/>.</param>
    /// <returns>The newly created branch, or <see langword="null"/> if creation failed.</returns>
    /// <exception cref="GitBranchNotExistsException">The source branch does not exist.</exception>
    public static IGitBranch? CreateBranch(this IGitRepository gitRepository, string sourceBranchName,
        string newBranchName, bool updateSourceBranchBefore)
    {
        var sourceBranch = gitRepository.Branches.CheckOut(sourceBranchName);

        if (sourceBranch == null)
        {
            throw new GitBranchNotExistsException(sourceBranchName);
        }

        if (updateSourceBranchBefore)
        {
            gitRepository.FetchAllTagsFromOrigin();

            gitRepository.Pull();
        }

        return gitRepository.Branches.CreateBranch(newBranchName);
    }

    /// <summary>
    /// Determines whether the branch has been pushed to its tracked remote branch.
    /// </summary>
    /// <param name="branch">The branch to check.</param>
    /// <returns><see langword="true"/> if the branch is remote or its tip matches the tracked branch tip; otherwise, <see langword="false"/>.</returns>
    public static bool BranchIsPushedToRemote(this IGitBranch branch)
    {
        return branch.IsRemote || branch.Tip?.Sha == branch.TrackedBranch?.Tip?.Sha;
    }

    /// <summary>
    /// Enumerates the commits on the branch that have not yet been pushed to the tracked remote branch.
    /// </summary>
    /// <param name="branch">The branch whose unpushed commits to enumerate.</param>
    /// <returns>A sequence of commits that exist locally but not on the tracked remote branch.</returns>
    public static IEnumerable<IGitCommit> UnPushedCommits(this IGitBranch branch)
    {
        if (branch.TrackedBranch == null || branch.Commits == null)
        {
            yield break;
        }

        foreach (var gitCommit in branch.Commits)
        {
            if (gitCommit.Id.Equals(branch.TrackedBranch.Tip?.Id))
            {
                yield break;
            }

            yield return gitCommit;
        }
    }
}
