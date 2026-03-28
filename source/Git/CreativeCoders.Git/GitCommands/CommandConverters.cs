using CreativeCoders.Git.Abstractions.Pushes;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

/// <summary>
/// Provides extension methods for converting LibGit2Sharp command types to their Git abstraction equivalents.
/// </summary>
public static class CommandConverters
{
    /// <summary>
    /// Converts a LibGit2Sharp <see cref="PackBuilderStage"/> to a <see cref="GitPackBuilderStage"/>.
    /// </summary>
    /// <param name="stage">The LibGit2Sharp pack builder stage.</param>
    /// <returns>The corresponding <see cref="GitPackBuilderStage"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a recognized pack builder stage.</exception>
    public static GitPackBuilderStage ToGitPackBuilderStage(this PackBuilderStage stage)
    {
        return stage switch
        {
            PackBuilderStage.Counting => GitPackBuilderStage.Counting,
            PackBuilderStage.Deltafying => GitPackBuilderStage.Deltafying,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
    }
}
