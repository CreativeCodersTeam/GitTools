namespace CreativeCoders.Git.Abstractions;

public class GitMergeOptions
{
    public GitMergeOptions()
    {
        FindRenames = true;
        RenameThreshold = 50;
        TargetLimit = 200;
        CommitOnSuccess = true;
    }

    public GitFastForwardStrategy FastForwardStrategy { get; set; }

    /// <summary>
    /// Commit the merge if the merge is successful and this is a non-fast-forward merge.
    /// If this is a fast-forward merge, then there is no merge commit and this option
    /// will not affect the merge.
    /// </summary>
    public bool CommitOnSuccess { get; set; }

    /// <summary>
    /// How conflicting index entries should be written out during checkout.
    /// </summary>
    public GitCheckoutFileConflictStrategy FileConflictStrategy { get; set; }

    /// <summary>
    /// Find renames. Default is true.
    /// </summary>
    public bool FindRenames { get; set; }

    /// <summary>
    /// If set, do not create or return conflict entries, but stop and return
    /// an error result after finding the first conflict.
    /// </summary>
    public bool FailOnConflict { get; set; }

    /// <summary>
    /// Do not write the Resolve Undo Cache extension on the generated index. This can
    /// be useful when no merge resolution will be presented to the user (e.g. a server-side
    /// merge attempt).
    /// </summary>
    public bool SkipReuc { get; set; }

    /// <summary>
    /// Similarity to consider a file renamed.
    /// </summary>
    public int RenameThreshold;

    /// <summary>
    /// Maximum similarity sources to examine (overrides
    /// 'merge.renameLimit' config (default 200)
    /// </summary>
    public int TargetLimit;

    /// <summary>
    /// How to handle conflicts encountered during a merge.
    /// </summary>
    public GitMergeFileFavor MergeFileFavor { get; set; }

    /// <summary>
    /// Ignore changes in amount of whitespace
    /// </summary>
    public bool IgnoreWhitespaceChange { get; set; }
}