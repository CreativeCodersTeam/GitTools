namespace CreativeCoders.Git.Abstractions.Commits;

public enum GitMergeStatus
{
    /// <summary>Merge was up-to-date.</summary>
    UpToDate,

    /// <summary>Fast-forward merge.</summary>
    FastForward,

    /// <summary>Non-fast-forward merge.</summary>
    NonFastForward,

    /// <summary>Merge resulted in conflicts.</summary>
    Conflicts
}
