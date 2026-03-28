namespace CreativeCoders.Git.Abstractions.Fetches;

/// <summary>
/// Represents the progress of a Git fetch transfer operation.
/// </summary>
public class GitFetchTransferProgress
{
    /// <summary>
    /// Gets the number of objects that have been indexed.
    /// </summary>
    public int IndexedObjects { get; init; }

    /// <summary>
    /// Gets the number of objects that have been received.
    /// </summary>
    public int ReceivedObjects { get; init; }

    /// <summary>
    /// Gets the total number of objects to be transferred.
    /// </summary>
    public int TotalObjects { get; init; }

    /// <summary>
    /// Gets the number of bytes received so far.
    /// </summary>
    public long ReceivedBytes { get; init; }
}
