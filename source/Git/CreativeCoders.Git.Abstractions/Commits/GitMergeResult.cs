namespace CreativeCoders.Git.Abstractions.Commits;

public class GitMergeResult
{
    public GitMergeResult(GitMergeStatus mergeStatus, IGitCommit? commit)
    {
        MergeStatus = mergeStatus;
        Commit = commit;
    }

    public GitMergeStatus MergeStatus { get; }

    public IGitCommit? Commit { get; }
}