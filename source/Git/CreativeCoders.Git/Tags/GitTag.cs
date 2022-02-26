using CreativeCoders.Core;
using CreativeCoders.Core.Comparing;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.Git.Commits;
using LibGit2Sharp;

namespace CreativeCoders.Git.Tags;

public class GitTag : ComparableObject<GitTag, IGitTag>, IGitTag
{
    private readonly Tag _tag;

    internal GitTag(Tag tag)
    {
        _tag = Ensure.NotNull(tag, nameof(tag));

        Name = new ReferenceName(_tag.CanonicalName);
    }

    static GitTag() => InitComparableObject(x => x.Name.Canonical);

    public ReferenceName Name { get; }

    public string TargetSha => _tag.Target.Sha;

    public IGitCommit? PeeledTargetCommit()
    {
        var target = _tag.Target;

        while (target is TagAnnotation annotation)
        {
            target = annotation.Target;
        }

        return GitCommit.From(target as Commit);
    }

    public static implicit operator Tag(GitTag tag) => tag._tag;
}