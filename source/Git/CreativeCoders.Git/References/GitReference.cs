using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.References;

public class GitReference : ComparableObject<GitReference, IGitReference>, IGitReference
{
    private readonly Reference _reference;

    internal GitReference(Reference reference)
    {
        _reference = Ensure.NotNull(reference, nameof(reference));

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

    public ReferenceName Name { get; }

    public string TargetIdentifier => _reference.TargetIdentifier;

    public IGitObjectId? ReferenceTargetId { get; }

    public static implicit operator Reference(GitReference reference) => reference._reference;
}