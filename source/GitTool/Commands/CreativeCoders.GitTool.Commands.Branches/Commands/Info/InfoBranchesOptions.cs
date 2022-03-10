using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

public class InfoBranchesOptions
{
    [OptionParameter('c', "commits", HelpText = "Max commit log entries listed", DefaultValue = 10)]
    public int CommitLogCount { get; set; }
}
