using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Common;

namespace CreativeCoders.Git.Commits;

/// <summary>
/// Represents a Git commit containing author, committer, message, and parent information.
/// </summary>
public sealed class GitCommit : Objects.GitObject, IGitCommit
{
    private readonly Commit _commit;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitCommit"/> class.
    /// </summary>
    /// <param name="commit">The underlying LibGit2Sharp commit.</param>
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

    /// <inheritdoc />
    public bool Equals(IGitCommit? other)
    {
        return base.Equals(other);
    }

    /// <inheritdoc />
    public int CompareTo(IGitCommit? other)
    {
        return base.CompareTo(other);
    }

    /// <inheritdoc />
    public IEnumerable<IGitCommit> Parents { get; }

    /// <inheritdoc />
    public IGitSignature Author { get; }

    /// <inheritdoc />
    public IGitSignature Committer { get; }

    /// <inheritdoc />
    public string Message => _commit.Message;

    /// <summary>
    /// Converts a <see cref="GitCommit"/> to a LibGit2Sharp <see cref="Commit"/>.
    /// </summary>
    /// <param name="commit">The Git commit to convert.</param>
    public static implicit operator Commit(GitCommit commit) => commit._commit;
}
