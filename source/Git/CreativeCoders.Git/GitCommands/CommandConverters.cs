using CreativeCoders.Git.Abstractions.Pushes;
using LibGit2Sharp.Handlers;

namespace CreativeCoders.Git.GitCommands;

public static class CommandConverters
{
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
