using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.RefSpecs;

/// <summary>
/// Represents a Git refspec that maps source references to destination references.
/// </summary>
[PublicAPI]
public interface IGitRefSpec : IEquatable<IGitRefSpec?>, IComparable<IGitRefSpec>
{
    /// <summary>
    /// Gets the full refspec specification string.
    /// </summary>
    string Specification { get; }

    /// <summary>
    /// Gets the direction of the refspec.
    /// </summary>
    GitRefSpecDirection Direction { get; }

    /// <summary>
    /// Gets the source part of the refspec.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Gets the destination part of the refspec.
    /// </summary>
    string Destination { get; }
}