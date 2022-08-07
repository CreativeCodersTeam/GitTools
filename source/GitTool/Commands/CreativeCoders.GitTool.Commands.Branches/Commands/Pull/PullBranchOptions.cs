using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public class PullBranchOptions
{
    [OptionParameter('v', "verbose")]
    public bool Verbose { get; set; }
}
