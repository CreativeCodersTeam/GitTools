﻿using CreativeCoders.Git.Abstractions.RefSpecs;

namespace CreativeCoders.Git.RefSpecs;

public class GitRefSpecCollection : IGitRefSpecCollection
{
    private readonly RefSpecCollection _refSpecCollection;

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