using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

[PublicAPI]
public class PushBranchOptions
{
    private const string CreateRemoteLongName = "createremote";

    [OptionParameter('b', CreateRemoteLongName, HelpText = "Creates a new remote branch if not tracking branch exits")]
    public bool CreateRemoteBranchIfNotExists { get; set; }

    [OptionParameter('c', "confirm", HelpText = "Confirm push if remote branch exists and there are commits to push")]
    public bool ConfirmPush { get; set; }

    [OptionParameter('v', "verbose")]
    public bool Verbose { get; set; }
}
