using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;

/// <summary>
/// Determines the local branches which have no matching counterpart on the remote repository.
/// </summary>
/// <param name="gitRepository">The repository whose branches are examined.</param>
public class OrphanedLocalBranchesProvider(IGitRepository gitRepository) : IOrphanedLocalBranchesProvider
{
    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    /// <inheritdoc />
    public IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches()
    {
        var currentBranch = _gitRepository.Head;

        var branches = _gitRepository.Branches.ToArray();

        var remoteFriendlyNames = branches
            .Where(x => x.IsRemote)
            .Select(x => x.Name.Friendly)
            .ToArray();

        var trackedBranchNames = _gitRepository.Remotes
            .SelectMany(remote => remoteFriendlyNames
                .Where(x => x.StartsWith($"{remote.Name}/", StringComparison.OrdinalIgnoreCase))
                .Select(x => x[(remote.Name.Length + 1)..]))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return branches
            .Where(x => !x.IsRemote
                        && !x.Equals(currentBranch)
                        && !trackedBranchNames.Contains(x.Name.Friendly))
            .ToArray();
    }
}
