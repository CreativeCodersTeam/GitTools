using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["branch"], "Commands for branches")]

namespace CreativeCoders.GitTool.Cli.Commands.Branches;

public static class BranchesCommandGroup
{
    public const string Name = "branch";
}
