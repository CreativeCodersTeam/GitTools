using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs;

[PublicAPI]
public interface IGitTreeChanges : IEnumerable<IGitTreeEntryChanges>, IDisposable
{
    IEnumerable<IGitTreeEntryChanges> Added { get; }

    IEnumerable<IGitTreeEntryChanges> Deleted { get; }

    IEnumerable<IGitTreeEntryChanges> Modified { get; }

    IEnumerable<IGitTreeEntryChanges> TypeChanged { get; }

    IEnumerable<IGitTreeEntryChanges> Renamed { get; }

    IEnumerable<IGitTreeEntryChanges> Copied { get; }

    IEnumerable<IGitTreeEntryChanges> Unmodified { get; }

    IEnumerable<IGitTreeEntryChanges> Conflicted { get; }

    int Count { get; }
}