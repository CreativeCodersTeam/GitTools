using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.References;

/// <summary>
/// Represents a collection of Git references with operations for adding, updating, and querying.
/// </summary>
public class GitReferenceCollection : IGitReferenceCollection
{
    private readonly ReferenceCollection _referenceCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitReferenceCollection"/> class.
    /// </summary>
    /// <param name="referenceCollection">The underlying LibGit2Sharp reference collection.</param>
    internal GitReferenceCollection(ReferenceCollection referenceCollection)
    {
        _referenceCollection = Ensure.NotNull(referenceCollection);
    }

    public IEnumerator<IGitReference> GetEnumerator()
        => _referenceCollection.Select(x => new GitReference(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IGitReference? Head => this["HEAD"];

    /// <inheritdoc />
    public IGitReference? this[string name]
        => GitReference.From(_referenceCollection[name]);

    /// <inheritdoc />
    public void Add(string name, string canonicalRefNameOrObjectish, bool allowOverwrite = false)
        => _referenceCollection.Add(name, canonicalRefNameOrObjectish, allowOverwrite);

    /// <inheritdoc />
    public void UpdateTarget(IGitReference directRef, IGitObjectId targetId)
        => new ActionExceptionWrapper<LockedFileException>(() =>
                _referenceCollection.UpdateTarget((GitReference)directRef, (GitObjectId)targetId))
            .Wrap(x => new GitLockedFileException(x))
            .Execute();

    /// <inheritdoc />
    public IEnumerable<IGitReference> FromGlob(string prefix)
        => _referenceCollection.FromGlob(prefix).Select(x => new GitReference(x));
}
