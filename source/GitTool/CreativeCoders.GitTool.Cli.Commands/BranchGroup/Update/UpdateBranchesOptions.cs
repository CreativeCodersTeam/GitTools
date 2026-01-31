using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.Update;

public class UpdateBranchesOptions
{
    [UsedImplicitly] public bool SkipFetchPrune { get; set; }
}
