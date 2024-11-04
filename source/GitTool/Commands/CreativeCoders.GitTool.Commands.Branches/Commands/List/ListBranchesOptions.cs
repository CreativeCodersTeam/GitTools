using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.List;

[PublicAPI]
public class ListBranchesOptions
{
    [OptionParameter('l', "Location", HelpText = "Location of branches (all, local, remote)")]
    public BranchLocation Location { get; set; }
}
