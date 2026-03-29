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
        Ensure.NotNull(commitFilter);

        if (_commitLog is not IQueryableCommitLog queryableCommitLog)
        {
            return Enumerable.Empty<IGitCommit>();
        }

        var filter = new CommitFilter
        {
            FirstParentOnly = commitFilter.FirstParentOnly,
            SortBy = (CommitSortStrategies)(int)commitFilter.SortBy,
            IncludeReachableFrom = MapReachableFrom(commitFilter.IncludeReachableFrom),
            ExcludeReachableFrom = MapReachableFrom(commitFilter.ExcludeReachableFrom)
        };

        IEnumerable<IGitCommit> results = queryableCommitLog.QueryBy(filter).Select(x => new GitCommit(x));

        if (commitFilter.After.HasValue)
        {
            results = results.Where(c => c.Committer.When >= commitFilter.After.Value);
        }

        if (commitFilter.Before.HasValue)
        {
            results = results.Where(c => c.Committer.When <= commitFilter.Before.Value);
        }

        if (commitFilter.AuthorEmail is not null)
        {
            results = commitFilter.AuthorEmailExactMatch
                ? results.Where(c =>
                    c.Author.Email.Equals(commitFilter.AuthorEmail, StringComparison.OrdinalIgnoreCase))
                : results.Where(c =>
                    c.Author.Email.Contains(commitFilter.AuthorEmail, StringComparison.OrdinalIgnoreCase));
        }

        if (commitFilter.MessagePattern is not null)
        {
            var regex = new Regex(commitFilter.MessagePattern, RegexOptions.IgnoreCase);
            results = results.Where(c => regex.IsMatch(c.Message));
        }

        if (commitFilter.MaxCount > 0)
        {
            results = results.Take(commitFilter.MaxCount);
        }

        return results;
    }

    private static object? MapReachableFrom(object? value)
    {
        return value switch
        {
            GitCommit gitCommit => (Commit)gitCommit,
            IEnumerable<IGitCommit> commits => commits.OfType<GitCommit>().Select(c => (Commit)c).ToList(),
            _ => value
        };
    }
}
