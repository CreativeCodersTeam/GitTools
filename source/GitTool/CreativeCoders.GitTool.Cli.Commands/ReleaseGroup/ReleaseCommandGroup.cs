using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["release"], "Commands for managing releases")]

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup;

public static class ReleaseCommandGroup
{
    public const string Name = "release";
}
