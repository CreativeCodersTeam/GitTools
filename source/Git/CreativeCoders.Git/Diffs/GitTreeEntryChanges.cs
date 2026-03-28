using CreativeCoders.Git.Abstractions.Diffs;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.Diffs;

/// <summary>
/// Represents the changes to a single entry in a Git tree diff comparison.
/// </summary>
public class GitTreeEntryChanges : IGitTreeEntryChanges
{
    private readonly TreeEntryChanges _treeEntryChanges;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitTreeEntryChanges"/> class.
    /// </summary>
    /// <param name="treeEntryChanges">The underlying LibGit2Sharp tree entry changes.</param>
    internal GitTreeEntryChanges(TreeEntryChanges treeEntryChanges)
    {
        _treeEntryChanges = Ensure.NotNull(treeEntryChanges);

        Mode = treeEntryChanges.Mode.ToGitEntryMode();
        Oid = new GitObjectId(treeEntryChanges.Oid);
        Status = treeEntryChanges.Status.ToGitEntryChangeKind();
        OldMode = treeEntryChanges.OldMode.ToGitEntryMode();
        OldOid = new GitObjectId(treeEntryChanges.OldOid);
    }

    /// <inheritdoc />
    public string Path => _treeEntryChanges.Path;

    /// <inheritdoc />
    public GitEntryMode Mode { get; }

    /// <inheritdoc />
    public IGitObjectId Oid { get; }

    /// <inheritdoc />
    public bool Exists => _treeEntryChanges.Exists;

    /// <inheritdoc />
    public GitEntryChangeKind Status { get; }

    /// <inheritdoc />
    public string OldPath => _treeEntryChanges.OldPath;

    /// <inheritdoc />
    public GitEntryMode OldMode { get; }

    /// <inheritdoc />
    public IGitObjectId OldOid { get; }

    /// <inheritdoc />
    public bool OldExists => _treeEntryChanges.OldExists;
}
