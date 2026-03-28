using CreativeCoders.Git.Abstractions.Commits;

namespace CreativeCoders.Git.Commits;

/// <summary>
/// Represents an enumerable log of Git commits.
/// </summary>
public class GitCommitLog : IGitCommitLog
{
    private readonly ICommitLog _commitLog;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitCommitLog"/> class.
    /// </summary>
    /// <param name="commitLog">The underlying LibGit2Sharp commit log.</param>
    internal GitCommitLog(ICommitLog commitLog)
    {
        _commitLog = Ensure.NotNull(commitLog);
    }

    internal static GitCommitLog? From(ICommitLog? commitLog)
    {
        return commitLog == null
            ? null
            : new GitCommitLog(commitLog);
    }

    public IEnumerator<IGitCommit> GetEnumerator()
        => _commitLog.Select(x => new GitCommit(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IEnumerable<IGitCommit> GetCommitsPriorTo(DateTimeOffset olderThan)
        => this.SkipWhile(c => c.Committer.When > olderThan);

    /// <inheritdoc />
    public IEnumerable<IGitCommit> QueryBy(GitCommitFilter commitFilter)
    {
        throw new NotImplementedException();
    }
}
