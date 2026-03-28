using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Objects;

/// <summary>
/// Represents a unique identifier for a Git object.
/// </summary>
[PublicAPI]
public interface IGitObjectId : IEquatable<IGitObjectId?>, IComparable<IGitObjectId>
{
    /// <summary>
    /// Gets the full SHA hash of the object.
    /// </summary>
    string Sha { get; }

    /// <summary>
    /// Returns the abbreviated SHA hash with the specified prefix length.
    /// </summary>
    /// <param name="prefixLength">The number of characters to include from the SHA hash.</param>
    /// <returns>The abbreviated SHA hash string.</returns>
    string ToString(int prefixLength);
}