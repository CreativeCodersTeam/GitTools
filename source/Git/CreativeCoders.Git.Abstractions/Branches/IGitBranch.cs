using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Branches
{
    [PublicAPI]
    public interface IGitBranch : IEquatable<IGitBranch>, IComparable<IGitBranch>, INamedReference
    {
        IGitCommit? Tip { get; }

        bool IsRemote { get; }

        bool IsTracking { get; }

        IGitBranch? TrackedBranch { get; }

        bool IsDetachedHead { get; }

        IGitCommitLog? Commits { get; }
    }
}
