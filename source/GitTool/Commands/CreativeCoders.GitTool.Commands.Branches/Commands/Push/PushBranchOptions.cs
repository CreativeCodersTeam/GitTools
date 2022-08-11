using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

public class PushBranchOptions
{
    private const string CreateRemoteLongName = "createremote";

    [OptionParameter('c', CreateRemoteLongName)]
    public bool CreateRemoteBranchIfNotExists { get; set; }

    [OptionParameter('v', "verbose")]
    public bool Verbose { get; set; }
}
