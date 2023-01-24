using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.CheckOut;

[UsedImplicitly]
public class CheckOutBranchesOptions
{
    [OptionValue(0, IsRequired = true)]
    public string BranchName { get; set; }
}
