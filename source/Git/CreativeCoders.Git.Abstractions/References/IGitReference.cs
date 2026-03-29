using System;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.References;

/// <summary>
/// Represents a Git reference pointing to a target object or another reference.
/// </summary>
[PublicAPI]
public interface IGitReference : IEquatable<IGitReference?>, IComparable<IGitReference>, INamedReference
{
    /// <summary>
    /// Gets the target identifier of the reference (a SHA or another reference name).
    /// </summary>
    string TargetIdentifier { get; }

    /// <summary>
    /// Gets the object ID that the reference ultimately points to.
    /// </summary>
    IGitObjectId? ReferenceTargetId { get; }
}