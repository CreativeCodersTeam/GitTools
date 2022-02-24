using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.List
{
    public class ListBranchesOptions
    {
        [OptionParameter('l', "Location", HelpText = "Location of branches (all, local, remote)")]
        public BranchLocation Location { get; set; }
    }
}
