namespace CreativeCoders.Git.Abstractions.Pushes;

/// <summary>
/// Represents the progress of the pack builder during a Git push operation.
/// </summary>
public class GitPackBuilderProgress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitPackBuilderProgress"/> class.
    /// </summary>
    /// <param name="stage">One of the enumeration values that specifies the current pack builder stage.</param>
    /// <param name="current">The number of objects processed so far.</param>
    /// <param name="total">The total number of objects to process.</param>
    public GitPackBuilderProgress(GitPackBuilderStage stage, int current, int total)
    {
        Stage = stage;
        Current = current;
        Total = total;
    }

    /// <summary>
    /// Gets the current pack builder stage.
    /// </summary>
    public GitPackBuilderStage Stage { get; }

    /// <summary>
    /// Gets the number of objects processed so far.
    /// </summary>
    public int Current { get; }

    /// <summary>
    /// Gets the total number of objects to process.
    /// </summary>
    public int Total { get; }
}
