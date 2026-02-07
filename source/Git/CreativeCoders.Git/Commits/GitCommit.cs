using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Common;

namespace CreativeCoders.Git.Commits;

public sealed class GitCommit : Objects.GitObject, IGitCommit
{
    private readonly Commit _commit;

    internal GitCommit(Commit commit) : base(commit)
    {
        _commit = Ensure.NotNull(commit);

        Parents = _commit.Parents.Select(x => new GitCommit(x));
        Author = new GitSignature(_commit.Author);
        Committer = new GitSignature(_commit.Committer);
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

    public IGitSignature Author { get; }

    public IGitSignature Committer { get; }

    public string Message => _commit.Message;

    public static implicit operator Commit(GitCommit commit) => commit._commit;
}
