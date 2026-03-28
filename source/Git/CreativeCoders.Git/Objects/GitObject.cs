using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Objects;

/// <summary>
/// Represents a Git object identified by its SHA hash.
/// </summary>
public class GitObject : ComparableObject<GitObject, IGitObject>, IGitObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitObject"/> class.
    /// </summary>
    /// <param name="gitObject">The underlying LibGit2Sharp Git object.</param>
    internal GitObject(LibGit2Sharp.GitObject gitObject)
    {
        Id = new GitObjectId(gitObject.Id);
        Sha = gitObject.Sha;
    }

    static GitObject() => InitComparableObject(x => x.Id);

    /// <inheritdoc />
    public IGitObjectId Id { get; }

    /// <inheritdoc />
    public string Sha { get; }
}