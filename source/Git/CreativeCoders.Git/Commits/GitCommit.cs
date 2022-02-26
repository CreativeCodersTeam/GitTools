using CreativeCoders.Git.Abstractions.Commits;

namespace CreativeCoders.Git.Commits;

public class GitCommit : Objects.GitObject, IGitCommit
{
    private readonly Commit _commit;

    internal GitCommit(Commit commit) : base(commit)
    {
        _commit = Ensure.NotNull(commit, nameof(commit));

        Parents = _commit.Parents.Select(x => new GitCommit(x));
    }

    internal static GitCommit? From(Commit? commit)
    {
        return commit == null
            ? null
            : new GitCommit(commit);
    }

    public bool Equals(IGitCommit? other)
    {
        return base.Equals(other);
    }

    public int CompareTo(IGitCommit? other)
    {
        return base.CompareTo(other);
    }

    public IEnumerable<IGitCommit> Parents { get; }

    public DateTimeOffset When => _commit.Committer.When;

    public string Message => _commit.Message;

    public static implicit operator Commit(GitCommit commit) => commit._commit;
}