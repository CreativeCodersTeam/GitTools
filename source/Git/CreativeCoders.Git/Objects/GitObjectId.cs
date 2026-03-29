using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Objects;

/// <summary>
/// Represents a unique identifier for a Git object based on its SHA hash.
/// </summary>
public class GitObjectId : ComparableObject<GitObjectId, IGitObjectId>, IGitObjectId
{
    private readonly ObjectId _objectId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitObjectId"/> class from a LibGit2Sharp <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="objectId">The underlying LibGit2Sharp object identifier.</param>
    internal GitObjectId(ObjectId objectId)
    {
        _objectId = Ensure.NotNull(objectId);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitObjectId"/> class from a SHA hash string.
    /// </summary>
    /// <param name="sha">The SHA hash string.</param>
    public GitObjectId(string sha) : this(new ObjectId(sha))
    {
    }

    static GitObjectId()
    {
        InitComparableObject(x => x.Sha);
    }

    /// <summary>
    /// Converts a <see cref="GitObjectId"/> to a LibGit2Sharp <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="objectId">The Git object identifier to convert.</param>
    public static implicit operator ObjectId(GitObjectId objectId) => objectId._objectId;

    /// <inheritdoc />
    public string Sha => _objectId.Sha;

    /// <inheritdoc />
    public string ToString(int prefixLength) => _objectId.ToString(prefixLength);
}
