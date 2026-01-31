using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Branches.Pull;

[PublicAPI]
public class PullBranchOptions
{
    [OptionParameter('v', "verbose")] public bool Verbose { get; set; }
}
