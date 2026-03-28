using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Tags;
using CreativeCoders.Git.Commits;

namespace CreativeCoders.Git.Tags;

/// <summary>
/// Represents a Git tag pointing to a specific Git object.
/// </summary>
public class GitTag : ComparableObject<GitTag, IGitTag>, IGitTag
{
    private readonly Tag _tag;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitTag"/> class.
    /// </summary>
    /// <param name="tag">The underlying LibGit2Sharp tag.</param>
    internal GitTag(Tag tag)
    {
        _tag = Ensure.NotNull(tag);

        Name = new ReferenceName(_tag.CanonicalName);

        if (_tag.Target is Commit commit)
        {
            TargetCommit = GitCommit.From(commit);
        }
    }

    static GitTag() => InitComparableObject(x => x.Name.Canonical);

    /// <inheritdoc />
    public ReferenceName Name { get; }

    /// <inheritdoc />
    public string TargetSha => _tag.Target.Sha;

    /// <inheritdoc />
    public IGitCommit? PeeledTargetCommit()
    {
        var target = _tag.Target;

        while (target is TagAnnotation annotation)
        {
            target = annotation.Target;
        }

        return GitCommit.From(target as Commit);
    }

    /// <inheritdoc />
    public IGitCommit? TargetCommit { get; }

    /// <summary>
    /// Converts a <see cref="GitTag"/> to a LibGit2Sharp <see cref="Tag"/>.
    /// </summary>
    /// <param name="tag">The Git tag to convert.</param>
    public static implicit operator Tag(GitTag tag) => tag._tag;
}
