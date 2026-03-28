namespace CreativeCoders.Git.Abstractions.Common;

/// <summary>
/// Represents a Git reference that has a <see cref="ReferenceName"/>.
/// </summary>
public interface INamedReference
{
    /// <summary>
    /// Gets the name of the reference.
    /// </summary>
    ReferenceName Name { get; }
}