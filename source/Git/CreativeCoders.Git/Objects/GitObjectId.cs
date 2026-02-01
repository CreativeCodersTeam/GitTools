using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Objects;

public class GitObjectId : ComparableObject<GitObjectId, IGitObjectId>, IGitObjectId
{
    private readonly ObjectId _objectId;

    internal GitObjectId(ObjectId objectId)
    {
        _objectId = Ensure.NotNull(objectId);
    }

    public GitObjectId(string sha) : this(new ObjectId(sha))
    {
    }

    static GitObjectId()
    {
        InitComparableObject(x => x.Sha);
    }

    public static implicit operator ObjectId(GitObjectId objectId) => objectId._objectId;

    public string Sha => _objectId.Sha;

    public string ToString(int prefixLength) => _objectId.ToString(prefixLength);
}
