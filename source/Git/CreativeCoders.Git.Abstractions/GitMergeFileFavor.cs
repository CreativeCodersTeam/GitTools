namespace CreativeCoders.Git.Abstractions;

public enum GitMergeFileFavor
{
    /// <summary>
    /// When a region of a file is changed in both branches, a conflict
    /// will be recorded in the index so that the checkout operation can produce
    /// a merge file with conflict markers in the working directory.
    /// This is the default.
    /// </summary>
    Normal,

    /// <summary>
    /// When a region of a file is changed in both branches, the file
    /// created in the index will contain the "ours" side of any conflicting
    /// region. The index will not record a conflict.
    /// </summary>
    Ours,

    /// <summary>
    /// When a region of a file is changed in both branches, the file
    /// created in the index will contain the "theirs" side of any conflicting
    /// region. The index will not record a conflict.
    /// </summary>
    Theirs,

    /// <summary>
    /// When a region of a file is changed in both branches, the file
    /// created in the index will contain each unique line from each side,
    /// which has the result of combining both files. The index will not
    /// record a conflict.
    /// </summary>
    Union,
}