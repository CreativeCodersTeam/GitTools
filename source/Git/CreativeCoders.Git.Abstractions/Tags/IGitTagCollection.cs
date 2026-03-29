using System.Collections.Generic;

namespace CreativeCoders.Git.Abstractions.Tags;

/// <summary>
/// Represents the collection of tags in a Git repository.
/// </summary>
public interface IGitTagCollection : IEnumerable<IGitTag>
{
    /// <summary>
    /// Creates a lightweight tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to create.</param>
    /// <param name="objectish">The target object SHA or reference, or <see langword="null"/> to tag HEAD.</param>
    /// <returns>The newly created tag.</returns>
    IGitTag CreateTag(string tagName, string? objectish = null);

    /// <summary>
    /// Creates an annotated tag with the specified name and message.
    /// </summary>
    /// <param name="tagName">The name of the tag to create.</param>
    /// <param name="message">The tag message.</param>
    /// <param name="objectish">The target object SHA or reference, or <see langword="null"/> to tag HEAD.</param>
    /// <returns>The newly created tag.</returns>
    IGitTag CreateTagWithMessage(string tagName, string message, string? objectish = null);

    /// <summary>
    /// Deletes the tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <param name="deleteOnRemote"><see langword="true"/> to also delete the tag on the remote; otherwise, <see langword="false"/>.</param>
    void DeleteTag(string tagName, bool deleteOnRemote = false);

    /// <summary>
    /// Deletes the specified tag.
    /// </summary>
    /// <param name="tag">The tag to delete.</param>
    /// <param name="deleteOnRemote"><see langword="true"/> to also delete the tag on the remote; otherwise, <see langword="false"/>.</param>
    void DeleteTag(IGitTag tag, bool deleteOnRemote = false);

    /// <summary>
    /// Deletes a tag on the remote repository.
    /// </summary>
    /// <param name="tagName">The name of the remote tag to delete.</param>
    void DeleteRemoteTag(string tagName);

    /// <summary>
    /// Pushes the tag with the specified name to the remote repository.
    /// </summary>
    /// <param name="tagName">The name of the tag to push.</param>
    void PushTag(string tagName);

    /// <summary>
    /// Pushes the specified tag to the remote repository.
    /// </summary>
    /// <param name="tag">The tag to push.</param>
    void PushTag(IGitTag tag);

    /// <summary>
    /// Pushes all local tags to the remote repository.
    /// </summary>
    void PushAllTags();
}
