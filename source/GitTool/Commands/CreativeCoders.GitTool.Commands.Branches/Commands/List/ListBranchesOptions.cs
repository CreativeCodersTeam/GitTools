using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.List
{
    public class ListBranchesOptions
    {
        [OptionParameter('l', "Location")]
        public BranchLocation Location { get; set; }
    }
}
