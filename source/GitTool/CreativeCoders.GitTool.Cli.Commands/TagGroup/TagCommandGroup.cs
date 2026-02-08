using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["tag"], "Commands for managing tags")]

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup;

public static class TagCommandGroup
{
    public const string Name = "tag";
}
