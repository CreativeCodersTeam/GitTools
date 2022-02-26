using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Branches;

[PublicAPI]
public interface IGitBranchCollection : IEnumerable<IGitBranch>
{
    IGitBranch? this[string name] { get; }
}