using System;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.References
{
    [PublicAPI]
    public interface IGitReference : IEquatable<IGitReference?>, IComparable<IGitReference>, INamedReference
    {
        string TargetIdentifier { get; }

        IGitObjectId? ReferenceTargetId { get; }
    }
}
