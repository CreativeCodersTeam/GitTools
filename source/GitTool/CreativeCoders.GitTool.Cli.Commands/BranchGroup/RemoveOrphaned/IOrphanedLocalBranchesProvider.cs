using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;

/// <summary>
/// Provides the local branches which have no matching counterpart on the remote repository.
/// </summary>
public interface IOrphanedLocalBranchesProvider
{
    /// <summary>
    /// Gets all local branches whose remote counterpart no longer exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <paramref name="includeUntracked" /> is <see langword="false" />, only local branches with a configured
    /// upstream are examined: a branch is orphaned when <see cref="IGitBranch.IsTracking" /> is
    /// <see langword="true" />, <see cref="IGitBranch.TrackedBranch" /> is not <see langword="null" /> and no
    /// remote branch with the canonical name of the tracked branch exists in the repository. Local branches
    /// without an upstream are never returned in this mode.
    /// </para>
    /// <para>
    /// If <paramref name="includeUntracked" /> is <see langword="true" />, the upstream configuration is ignored:
    /// a local branch is orphaned when no branch with the same friendly name exists on any remote.
    /// </para>
    /// <para>
    /// Name comparisons are case-insensitive in both modes. Remote branches are never returned, and the current
    /// HEAD branch is never returned, even if it qualifies as orphaned.
    /// </para>
    /// </remarks>
    /// <param name="includeUntracked">
    /// <see langword="true" /> to also report local branches without a configured upstream by matching branch
    /// names; <see langword="false" /> to report only branches whose configured upstream is missing.
    /// </param>
    /// <returns>The orphaned local branches, or an empty collection if there are none.</returns>
    IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches(bool includeUntracked);
}
