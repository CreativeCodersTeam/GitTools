using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Branches;
using LibGit2Sharp;

namespace CreativeCoders.Git.Branches;

public class GitBranchCollection : IGitBranchCollection
{
    private readonly BranchCollection _branchCollection;

    internal GitBranchCollection(BranchCollection branchCollection)
    {
        _branchCollection = Ensure.NotNull(branchCollection, nameof(branchCollection));
    }

    public IEnumerator<IGitBranch> GetEnumerator()
        => _branchCollection.Select(x => new GitBranch(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IGitBranch? this[string name] => GitBranch.From(_branchCollection[name]);
}