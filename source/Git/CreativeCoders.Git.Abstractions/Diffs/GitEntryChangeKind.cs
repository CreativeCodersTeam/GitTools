namespace CreativeCoders.Git.Abstractions.Diffs
{
    public enum GitEntryChangeKind
    {
        Unmodified,
        Added,
        Deleted,
        Modified,
        Renamed,
        Copied,
        Ignored,
        Untracked,
        TypeChanged,
        Unreadable,
        Conflicted,
    }
}
