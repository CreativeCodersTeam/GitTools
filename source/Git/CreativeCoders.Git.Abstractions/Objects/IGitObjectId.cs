using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Objects
{
    [PublicAPI]
    public interface IGitObjectId : IEquatable<IGitObjectId?>, IComparable<IGitObjectId>
    {
        string Sha { get; }

        string ToString(int prefixLength);
    }
}
