using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Objects;

namespace CreativeCoders.Git.Abstractions.Pushes;

public class GitPushUpdate
{
    public GitPushUpdate(IGitObjectId sourceObjectId, string sourceRefName, IGitObjectId destinationObjectId,
        string destinationRefName)
    {
        SourceObjectId = Ensure.NotNull(sourceObjectId);
        SourceRefName = sourceRefName;
        DestinationObjectId = Ensure.NotNull(destinationObjectId);
        DestinationRefName = destinationRefName;
    }

    public IGitObjectId SourceObjectId { get; }

    public string SourceRefName { get; }

    public IGitObjectId DestinationObjectId { get; }

    public string DestinationRefName { get; }
}
