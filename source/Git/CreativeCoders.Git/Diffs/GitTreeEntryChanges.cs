using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.Diffs;

public class GitTreeEntryChanges : IGitTreeEntryChanges
{
    private readonly TreeEntryChanges _treeEntryChanges;

    internal GitTreeEntryChanges(TreeEntryChanges treeEntryChanges)
    {
        _treeEntryChanges = Ensure.NotNull(treeEntryChanges);

        Mode = treeEntryChanges.Mode.ToGitEntryMode();
        Oid = new GitObjectId(treeEntryChanges.Oid);
        Status = treeEntryChanges.Status.ToGitEntryChangeKind();
        OldMode = treeEntryChanges.OldMode.ToGitEntryMode();
        OldOid = new GitObjectId(treeEntryChanges.OldOid);
    }

    public string Path => _treeEntryChanges.Path;

    public GitEntryMode Mode { get; }

    public IGitObjectId Oid { get; }

    public bool Exists => _treeEntryChanges.Exists;

    public GitEntryChangeKind Status { get; }

    public string OldPath => _treeEntryChanges.OldPath;

    public GitEntryMode OldMode { get; }

    public IGitObjectId OldOid { get; }

    public bool OldExists => _treeEntryChanges.OldExists;
}
