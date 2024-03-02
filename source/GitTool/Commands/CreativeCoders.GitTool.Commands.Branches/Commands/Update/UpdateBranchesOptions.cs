using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Update;

public class UpdateBranchesOptions
{
    [UsedImplicitly] public bool SkipFetchPrune { get; set; }
}
