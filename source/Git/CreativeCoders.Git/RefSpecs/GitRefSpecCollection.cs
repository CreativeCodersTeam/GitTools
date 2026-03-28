using CreativeCoders.Git.Abstractions.RefSpecs;

namespace CreativeCoders.Git.RefSpecs;

/// <summary>
/// Represents a collection of Git ref specs wrapping a LibGit2Sharp <see cref="RefSpecCollection"/>.
/// </summary>
public class GitRefSpecCollection : IGitRefSpecCollection
{
    private readonly RefSpecCollection _refSpecCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitRefSpecCollection"/> class.
    /// </summary>
    /// <param name="refSpecCollection">The underlying LibGit2Sharp ref spec collection.</param>
    public GitRefSpecCollection(RefSpecCollection refSpecCollection)
    {
        _refSpecCollection = refSpecCollection;
    }

    public IEnumerator<IGitRefSpec> GetEnumerator()
        => _refSpecCollection.Select(x => new GitRefSpec(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}