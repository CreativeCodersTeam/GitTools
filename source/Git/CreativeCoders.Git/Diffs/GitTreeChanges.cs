using CreativeCoders.Git.Abstractions.Diffs;

namespace CreativeCoders.Git.Diffs;

/// <summary>
/// Represents a set of tree changes resulting from a diff comparison, categorized by change type.
/// </summary>
public sealed class GitTreeChanges : IGitTreeChanges
{
    private readonly TreeChanges _treeChanges;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitTreeChanges"/> class.
    /// </summary>
    /// <param name="treeChanges">The underlying LibGit2Sharp tree changes.</param>
    internal GitTreeChanges(TreeChanges treeChanges)
    {
        _treeChanges = Ensure.NotNull(treeChanges);
    }

    public IEnumerator<IGitTreeEntryChanges> GetEnumerator()
        => _treeChanges.Select(x => new GitTreeEntryChanges(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _treeChanges.Dispose();
    }

    private static IEnumerable<IGitTreeEntryChanges> ToGitTreeEntryChanges(
        IEnumerable<TreeEntryChanges> treeEntryChanges)
        => treeEntryChanges.Select(x => new GitTreeEntryChanges(x));

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Added
        => ToGitTreeEntryChanges(_treeChanges.Added);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Deleted
        => ToGitTreeEntryChanges(_treeChanges.Deleted);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Modified
        => ToGitTreeEntryChanges(_treeChanges.Modified);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> TypeChanged
        => ToGitTreeEntryChanges(_treeChanges.TypeChanged);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Renamed
        => ToGitTreeEntryChanges(_treeChanges.Renamed);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Copied
        => ToGitTreeEntryChanges(_treeChanges.Copied);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Unmodified
        => ToGitTreeEntryChanges(_treeChanges.Unmodified);

    /// <inheritdoc />
    public IEnumerable<IGitTreeEntryChanges> Conflicted
        => ToGitTreeEntryChanges(_treeChanges.Conflicted);

    /// <inheritdoc />
    public int Count => _treeChanges.Count;
}
