using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["branch"], "Commands for branches")]

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup;

public static class BranchCommandGroup
{
    public const string Name = "branch";
}
