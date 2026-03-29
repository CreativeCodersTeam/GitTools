using CreativeCoders.Git.Abstractions.Diffs;

namespace CreativeCoders.Git.Diffs;

/// <summary>
/// Provides methods for comparing the working directory, index, and tree to produce diff results.
/// </summary>
public class GitDiffer : IGitDiffer
{
    private readonly Diff _diff;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitDiffer"/> class.
    /// </summary>
    /// <param name="diff">The underlying LibGit2Sharp diff engine.</param>
    public GitDiffer(Diff diff)
    {
        _diff = Ensure.NotNull(diff);
    }

    /// <inheritdoc />
    public IGitTreeChanges Compare()
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>());
    }

    /// <inheritdoc />
    public IGitTreeChanges Compare(bool includeUntracked)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(null, includeUntracked));
    }

    /// <inheritdoc />
    public IGitTreeChanges Compare(IEnumerable<string> paths)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(paths));
    }

    /// <inheritdoc />
    public IGitTreeChanges Compare(IEnumerable<string> paths, bool includeUntracked)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(paths, includeUntracked));
    }
}
