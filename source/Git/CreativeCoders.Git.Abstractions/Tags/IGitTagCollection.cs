using System.Collections.Generic;

namespace CreativeCoders.Git.Abstractions.Tags;

public interface IGitTagCollection : IEnumerable<IGitTag>
{
    IGitTag CreateTag(string tagName, string? objectish = null);

    IGitTag CreateTagWithMessage(string tagName, string message, string? objectish = null);

    void PushTag(IGitTag tag);

    void PushAllTags();
}
