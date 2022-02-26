using CreativeCoders.Core.Comparing;
using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Objects;

public class GitObject : ComparableObject<GitObject, IGitObject>, IGitObject
{
    internal GitObject(LibGit2Sharp.GitObject gitObject)
    {
        Id = new GitObjectId(gitObject.Id);
        Sha = gitObject.Sha;
    }

    static GitObject() => InitComparableObject(x => x.Id);

    public IGitObjectId Id { get; }

    public string Sha { get; }
}