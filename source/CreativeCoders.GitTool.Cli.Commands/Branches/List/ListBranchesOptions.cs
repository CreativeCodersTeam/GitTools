using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.List;

[PublicAPI]
public class ListBranchesOptions
{
    [OptionParameter('l', "Location", HelpText = "Location of branches (all, local, remote)")]
    public BranchLocation Location { get; set; }
}
