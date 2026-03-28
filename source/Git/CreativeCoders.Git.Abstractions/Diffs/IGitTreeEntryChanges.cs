using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Diffs;

/// <summary>
/// Represents the changes to a single entry in a Git tree diff.
/// </summary>
[PublicAPI]
public interface IGitTreeEntryChanges
{
    /// <summary>
    /// Gets the current path of the entry.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the current file mode of the entry.
    /// </summary>
    GitEntryMode Mode { get; }

    /// <summary>
    /// Gets the current object ID of the entry.
    /// </summary>
    IGitObjectId Oid { get; }

    /// <summary>
    /// Gets a value that indicates whether the entry exists in the new tree.
    /// </summary>
    /// <value><see langword="true"/> if the entry exists; otherwise, <see langword="false"/>.</value>
    bool Exists { get; }

    /// <summary>
    /// Gets the kind of change applied to the entry.
    /// </summary>
    GitEntryChangeKind Status { get; }

    /// <summary>
    /// Gets the previous path of the entry before the change.
    /// </summary>
    string OldPath { get; }

    /// <summary>
    /// Gets the previous file mode of the entry.
    /// </summary>
    GitEntryMode OldMode { get; }

    /// <summary>
    /// Gets the previous object ID of the entry.
    /// </summary>
    IGitObjectId OldOid { get; }

    /// <summary>
    /// Gets a value that indicates whether the entry existed in the old tree.
    /// </summary>
    /// <value><see langword="true"/> if the entry existed; otherwise, <see langword="false"/>.</value>
    bool OldExists { get; }
}