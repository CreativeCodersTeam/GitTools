using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Objects;

/// <summary>
/// Represents a Git object with an identity and SHA hash.
/// </summary>
[PublicAPI]
public interface IGitObject : IEquatable<IGitObject?>, IComparable<IGitObject>
{
    /// <summary>
    /// Gets the object identifier.
    /// </summary>
    IGitObjectId Id { get; }

    /// <summary>
    /// Gets the full SHA hash of the object.
    /// </summary>
    string Sha { get; }
}