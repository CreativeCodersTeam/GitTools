using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Common;

/// <summary>
/// Represents the identity and timestamp of a Git commit author or committer.
/// </summary>
[PublicAPI]
public interface IGitSignature
{
    /// <summary>
    /// Gets the date and time when the action was performed.
    /// </summary>
    public DateTimeOffset When { get; }

    /// <summary>
    /// Gets the name of the person.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the email address of the person.
    /// </summary>
    public string Email { get; }
}
