using System.Collections.Generic;

namespace CreativeCoders.Git.Abstractions.RefSpecs;

/// <summary>
/// Represents a collection of <see cref="IGitRefSpec"/> instances.
/// </summary>
public interface IGitRefSpecCollection : IEnumerable<IGitRefSpec>;
