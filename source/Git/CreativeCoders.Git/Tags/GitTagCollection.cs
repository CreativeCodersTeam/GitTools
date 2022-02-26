using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Tags;
using LibGit2Sharp;

namespace CreativeCoders.Git.Tags;

public class GitTagCollection : IGitTagCollection
{
    private readonly TagCollection _tagCollection;

    internal GitTagCollection(TagCollection tagCollection)
    {
        _tagCollection = Ensure.NotNull(tagCollection, nameof(tagCollection));
    }

    public IEnumerator<IGitTag> GetEnumerator()
        => _tagCollection.Select(x => new GitTag(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}