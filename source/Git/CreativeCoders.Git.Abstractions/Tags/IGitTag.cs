using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Common;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Tags;

[PublicAPI]
public interface IGitTag : IEquatable<IGitTag?>, IComparable<IGitTag>, INamedReference
{
    string TargetSha { get; }

    IGitCommit? PeeledTargetCommit();
}