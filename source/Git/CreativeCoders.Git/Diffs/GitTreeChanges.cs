using CreativeCoders.Git.Abstractions.Diffs;

namespace CreativeCoders.Git.Diffs;

public sealed class GitTreeChanges : IGitTreeChanges
{
    private readonly TreeChanges _treeChanges;

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

    public void Dispose()
    {
        _treeChanges.Dispose();
    }

    private static IEnumerable<IGitTreeEntryChanges> ToGitTreeEntryChanges(
        IEnumerable<TreeEntryChanges> treeEntryChanges)
        => treeEntryChanges.Select(x => new GitTreeEntryChanges(x));

    public IEnumerable<IGitTreeEntryChanges> Added
        => ToGitTreeEntryChanges(_treeChanges.Added);

    public IEnumerable<IGitTreeEntryChanges> Deleted
        => ToGitTreeEntryChanges(_treeChanges.Deleted);

    public IEnumerable<IGitTreeEntryChanges> Modified
        => ToGitTreeEntryChanges(_treeChanges.Modified);

    public IEnumerable<IGitTreeEntryChanges> TypeChanged
        => ToGitTreeEntryChanges(_treeChanges.TypeChanged);

    public IEnumerable<IGitTreeEntryChanges> Renamed
        => ToGitTreeEntryChanges(_treeChanges.Renamed);

    public IEnumerable<IGitTreeEntryChanges> Copied
        => ToGitTreeEntryChanges(_treeChanges.Copied);

    public IEnumerable<IGitTreeEntryChanges> Unmodified
        => ToGitTreeEntryChanges(_treeChanges.Unmodified);

    public IEnumerable<IGitTreeEntryChanges> Conflicted
        => ToGitTreeEntryChanges(_treeChanges.Conflicted);

    public int Count => _treeChanges.Count;
}
