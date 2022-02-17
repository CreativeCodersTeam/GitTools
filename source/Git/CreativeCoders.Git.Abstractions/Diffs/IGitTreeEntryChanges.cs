using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs
{
    [PublicAPI]
    public interface IGitTreeEntryChanges
    {
        string Path { get; }

        GitEntryMode Mode { get; }

        IGitObjectId Oid { get; }

        bool Exists { get; }

        GitEntryChangeKind Status { get; }

        string OldPath { get; }

        GitEntryMode OldMode { get; }

        IGitObjectId OldOid { get; }

        bool OldExists { get; }
    }
}
