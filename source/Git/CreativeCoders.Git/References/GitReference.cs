using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.References;

/// <summary>
/// Represents a Git reference (e.g., branch, tag, or HEAD) pointing to a Git object.
/// </summary>
public class GitReference : ComparableObject<GitReference, IGitReference>, IGitReference
{
    private readonly Reference _reference;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitReference"/> class.
    /// </summary>
    /// <param name="reference">The underlying LibGit2Sharp reference.</param>
    internal GitReference(Reference reference)
    {
        _reference = Ensure.NotNull(reference);

        Name = new ReferenceName(reference.CanonicalName);

        if (reference is DirectReference)
        {
            ReferenceTargetId = new GitObjectId(reference.TargetIdentifier);
        }
    }

    static GitReference() => InitComparableObject(x => x.Name.Canonical);

    internal static GitReference? From(Reference? reference)
    {
        return reference == null
            ? null
            : new GitReference(reference);
    }

    /// <inheritdoc />
    public ReferenceName Name { get; }

    /// <inheritdoc />
    public string TargetIdentifier => _reference.TargetIdentifier;

    /// <inheritdoc />
    public IGitObjectId? ReferenceTargetId { get; }

    /// <summary>
    /// Converts a <see cref="GitReference"/> to a LibGit2Sharp <see cref="Reference"/>.
    /// </summary>
    /// <param name="reference">The Git reference to convert.</param>
    public static implicit operator Reference(GitReference reference) => reference._reference;
}
