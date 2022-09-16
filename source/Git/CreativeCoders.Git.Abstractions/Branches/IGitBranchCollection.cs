using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Branches;

[PublicAPI]
public interface IGitBranchCollection : IEnumerable<IGitBranch>
{
    IGitBranch? CheckOut(string branchName);

    IGitBranch? CreateBranch(string branchName);

    void DeleteLocalBranch(string branchName);

    IGitBranch? this[string name] { get; }
}