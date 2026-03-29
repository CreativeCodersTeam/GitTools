using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Abstractions.Pushes;

/// <summary>
/// Represents the mapping of a source reference to a destination reference during a Git push operation.
/// </summary>
public class GitPushUpdate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushUpdate"/> class.
    /// </summary>
    /// <param name="sourceObjectId">The object ID of the source reference.</param>
    /// <param name="sourceRefName">The name of the source reference.</param>
    /// <param name="destinationObjectId">The object ID of the destination reference.</param>
    /// <param name="destinationRefName">The name of the destination reference.</param>
    public GitPushUpdate(IGitObjectId sourceObjectId, string sourceRefName, IGitObjectId destinationObjectId,
        string destinationRefName)
    {
        SourceObjectId = Ensure.NotNull(sourceObjectId);
        SourceRefName = sourceRefName;
        DestinationObjectId = Ensure.NotNull(destinationObjectId);
        DestinationRefName = destinationRefName;
    }

    /// <summary>
    /// Gets the object ID of the source reference.
    /// </summary>
    public IGitObjectId SourceObjectId { get; }

    /// <summary>
    /// Gets the name of the source reference.
    /// </summary>
    public string SourceRefName { get; }

    /// <summary>
    /// Gets the object ID of the destination reference.
    /// </summary>
    public IGitObjectId DestinationObjectId { get; }

    /// <summary>
    /// Gets the name of the destination reference.
    /// </summary>
    public string DestinationRefName { get; }
}
