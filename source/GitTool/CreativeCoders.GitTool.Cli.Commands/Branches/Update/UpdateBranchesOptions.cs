using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.Update;

public class UpdateBranchesOptions
{
    [UsedImplicitly] public bool SkipFetchPrune { get; set; }
}
