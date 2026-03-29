namespace CreativeCoders.Git.Abstractions.Commits;

/// <summary>
/// Represents the result of a Git merge operation.
/// </summary>
public class GitMergeResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitMergeResult"/> class.
    /// </summary>
    /// <param name="mergeStatus">The status of the merge operation.</param>
    /// <param name="commit">The merge commit, or <see langword="null"/> if no merge commit was created.</param>
    public GitMergeResult(GitMergeStatus mergeStatus, IGitCommit? commit)
    {
        MergeStatus = mergeStatus;
        Commit = commit;
    }

    /// <summary>
    /// Gets the status of the merge operation.
    /// </summary>
    public GitMergeStatus MergeStatus { get; }

    /// <summary>
    /// Gets the merge commit, if one was created.
    /// </summary>
    public IGitCommit? Commit { get; }
}