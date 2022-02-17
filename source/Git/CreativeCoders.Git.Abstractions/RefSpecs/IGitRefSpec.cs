using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.RefSpecs
{
    [PublicAPI]
    public interface IGitRefSpec : IEquatable<IGitRefSpec?>, IComparable<IGitRefSpec>
    {
        string Specification { get; }

        GitRefSpecDirection Direction { get; }

        string Source { get; }

        string Destination { get; }
    }
}
