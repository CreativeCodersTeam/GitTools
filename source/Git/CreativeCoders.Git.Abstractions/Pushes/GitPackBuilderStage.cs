namespace CreativeCoders.Git.Abstractions.Pushes;

/// <summary>
/// Specifies the stage of the pack builder during a push operation.
/// </summary>
public enum GitPackBuilderStage
{
    /// <summary>The pack builder is counting objects.</summary>
    Counting,
    /// <summary>The pack builder is computing deltas.</summary>
    Deltafying
}
