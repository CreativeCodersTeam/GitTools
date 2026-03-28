namespace CreativeCoders.Git.Abstractions.Diffs;

/// <summary>
/// Specifies the kind of change applied to a tree entry.
/// </summary>
public enum GitEntryChangeKind
{
    /// <summary>The entry was not modified.</summary>
    Unmodified,
    /// <summary>The entry was added.</summary>
    Added,
    /// <summary>The entry was deleted.</summary>
    Deleted,
    /// <summary>The entry was modified.</summary>
    Modified,
    /// <summary>The entry was renamed.</summary>
    Renamed,
    /// <summary>The entry was copied.</summary>
    Copied,
    /// <summary>The entry is ignored.</summary>
    Ignored,
    /// <summary>The entry is untracked.</summary>
    Untracked,
    /// <summary>The entry type was changed.</summary>
    TypeChanged,
    /// <summary>The entry is unreadable.</summary>
    Unreadable,
    /// <summary>The entry is in a conflicted state.</summary>
    Conflicted
}
