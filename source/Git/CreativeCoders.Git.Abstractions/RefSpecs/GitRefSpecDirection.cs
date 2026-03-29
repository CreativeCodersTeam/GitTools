namespace CreativeCoders.Git.Abstractions.RefSpecs;

/// <summary>
/// Specifies the direction of a Git refspec.
/// </summary>
public enum GitRefSpecDirection
{
    /// <summary>The refspec is used for fetch operations.</summary>
    Fetch,
    /// <summary>The refspec is used for push operations.</summary>
    Push
}