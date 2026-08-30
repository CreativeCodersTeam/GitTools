using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;

/// <summary>
/// Provides the local branches which have no matching counterpart on the remote repository.
/// </summary>
public interface IOrphanedLocalBranchesProvider
{
    /// <summary>
    /// Gets all local branches for which no branch with the same name exists on any remote.
    /// </summary>
    /// <remarks>The current HEAD branch is never returned, even if it has no remote counterpart.</remarks>
    /// <returns>The orphaned local branches, or an empty collection if there are none.</returns>
    IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches();
}
