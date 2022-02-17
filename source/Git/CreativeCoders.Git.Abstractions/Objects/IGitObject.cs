using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Objects
{
    [PublicAPI]
    public interface IGitObject : IEquatable<IGitObject?>, IComparable<IGitObject>
    {
        IGitObjectId Id { get; }

        string Sha { get; }
    }
}
