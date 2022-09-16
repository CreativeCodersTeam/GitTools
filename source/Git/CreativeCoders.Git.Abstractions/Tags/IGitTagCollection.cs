using System.Collections.Generic;

namespace CreativeCoders.Git.Abstractions.Tags;

public interface IGitTagCollection : IEnumerable<IGitTag>
{
    IGitTag CreateTag(string tagName);

    IGitTag CreateTag(string tagName, string objectish);

    IGitTag CreateTagWithMessage(string tagName, string message);

    IGitTag CreateTagWithMessage(string tagName, string objectish, string message);

    void PushTag(IGitTag tag);

    void PushAllTags();
}