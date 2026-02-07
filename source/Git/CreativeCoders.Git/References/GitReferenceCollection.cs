using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.Git.Abstractions.Objects;
using CreativeCoders.Git.Abstractions.References;
using CreativeCoders.Git.Common;
using CreativeCoders.Git.Objects;

namespace CreativeCoders.Git.References;

public class GitReferenceCollection : IGitReferenceCollection
{
    private readonly ReferenceCollection _referenceCollection;

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

    public IGitReference? Head => this["HEAD"];

    public IGitReference? this[string name]
        => GitReference.From(_referenceCollection[name]);

    public void Add(string name, string canonicalRefNameOrObjectish, bool allowOverwrite = false)
        => _referenceCollection.Add(name, canonicalRefNameOrObjectish, allowOverwrite);

    public void UpdateTarget(IGitReference directRef, IGitObjectId targetId)
        => new ActionExceptionWrapper<LockedFileException>(() =>
                _referenceCollection.UpdateTarget((GitReference)directRef, (GitObjectId)targetId))
            .Wrap(x => new GitLockedFileException(x))
            .Execute();

    public IEnumerable<IGitReference> FromGlob(string prefix)
        => _referenceCollection.FromGlob(prefix).Select(x => new GitReference(x));
}
