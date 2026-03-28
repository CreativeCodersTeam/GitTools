namespace CreativeCoders.Git.Abstractions.Pushes;

/// <summary>
/// Represents an error reported for a specific reference during a Git push operation.
/// </summary>
public class GitPushStatusError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushStatusError"/> class.
    /// </summary>
    /// <param name="reference">The name of the reference that failed.</param>
    /// <param name="message">The error message describing the failure.</param>
    public GitPushStatusError(string reference, string message)
    {
        Reference = reference;
        Message = message;
    }

    /// <summary>
    /// Gets the name of the reference that failed.
    /// </summary>
    public string Reference { get; }

    /// <summary>
    /// Gets the error message describing the failure.
    /// </summary>
    public string Message { get; }
}
