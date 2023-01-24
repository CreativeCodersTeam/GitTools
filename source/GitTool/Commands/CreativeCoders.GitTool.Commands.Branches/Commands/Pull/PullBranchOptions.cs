using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

[UsedImplicitly]
public class PullBranchOptions
{
    [OptionParameter('v', "verbose")]
    public bool Verbose { get; set; }
}
