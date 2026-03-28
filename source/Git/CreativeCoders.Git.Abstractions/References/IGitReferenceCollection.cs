using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.References;

/// <summary>
/// Represents the collection of references in a Git repository.
/// </summary>
[PublicAPI]
public interface IGitReferenceCollection : IEnumerable<IGitReference>
{
    /// <summary>
    /// Gets the HEAD reference.
    /// </summary>
    IGitReference? Head { get; }

    /// <summary>
    /// Gets the reference with the specified name.
    /// </summary>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The matching reference, or <see langword="null"/> if not found.</returns>
    IGitReference? this[string name] { get; }

    /// <summary>
    /// Adds a new reference with the specified name and target.
    /// </summary>
    /// <param name="name">The name of the reference to create.</param>
    /// <param name="canonicalRefNameOrObjectish">The canonical reference name or object SHA to point to.</param>
    /// <param name="allowOverwrite"><see langword="true"/> to overwrite an existing reference; otherwise, <see langword="false"/>.</param>
    void Add(string name, string canonicalRefNameOrObjectish, bool allowOverwrite = false);

    /// <summary>
    /// Updates the target of a direct reference to point to a new object.
    /// </summary>
    /// <param name="directRef">The direct reference to update.</param>
    /// <param name="targetId">The new target object ID.</param>
    void UpdateTarget(IGitReference directRef, IGitObjectId targetId);

    /// <summary>
    /// Returns all references whose names match the specified glob pattern.
    /// </summary>
    /// <param name="prefix">The glob pattern to match reference names against.</param>
    /// <returns>A sequence of matching references.</returns>
    IEnumerable<IGitReference> FromGlob(string prefix);
}