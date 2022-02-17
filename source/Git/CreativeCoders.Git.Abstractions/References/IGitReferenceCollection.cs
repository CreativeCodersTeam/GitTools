using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.References
{
    [PublicAPI]
    public interface IGitReferenceCollection : IEnumerable<IGitReference>
    {
        IGitReference? Head { get; }

        IGitReference? this[string name] { get; }

        void Add(string name, string canonicalRefNameOrObjectish, bool allowOverwrite = false);

        void UpdateTarget(IGitReference directRef, IGitObjectId targetId);

        IEnumerable<IGitReference> FromGlob(string prefix);
    }
}
