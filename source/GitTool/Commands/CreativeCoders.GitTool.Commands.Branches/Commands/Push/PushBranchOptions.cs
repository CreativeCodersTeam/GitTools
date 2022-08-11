using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

public class PushBranchOptions
{
    [OptionParameter('c', "createremote")]
    public bool CreateRemoteBranchIfNotExists { get; set; }

    [OptionParameter('v', "verbose")]
    public bool Verbose { get; set; }
}
