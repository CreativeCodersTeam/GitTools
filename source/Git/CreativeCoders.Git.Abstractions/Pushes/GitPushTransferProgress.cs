namespace CreativeCoders.Git.Abstractions.Pushes;

/// <summary>
/// Represents the progress of the data transfer during a Git push operation.
/// </summary>
public class GitPushTransferProgress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushTransferProgress"/> class.
    /// </summary>
    /// <param name="current">The number of objects transferred so far.</param>
    /// <param name="total">The total number of objects to transfer.</param>
    /// <param name="bytes">The number of bytes transferred so far.</param>
    public GitPushTransferProgress(int current, int total, long bytes)
    {
        Current = current;
        Total = total;
        Bytes = bytes;
    }

    /// <summary>
    /// Gets the number of objects transferred so far.
    /// </summary>
    public int Current { get; }

    /// <summary>
    /// Gets the total number of objects to transfer.
    /// </summary>
    public int Total { get; }

    /// <summary>
    /// Gets the number of bytes transferred so far.
    /// </summary>
    public long Bytes { get; }
}
