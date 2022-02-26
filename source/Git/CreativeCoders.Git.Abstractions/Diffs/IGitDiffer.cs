using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs;

[PublicAPI]
public interface IGitDiffer
{
    IGitTreeChanges Compare();

    IGitTreeChanges Compare(bool includeUntracked);

    IGitTreeChanges Compare(IEnumerable<string> paths);

    IGitTreeChanges Compare(IEnumerable<string> paths, bool includeUntracked);
}