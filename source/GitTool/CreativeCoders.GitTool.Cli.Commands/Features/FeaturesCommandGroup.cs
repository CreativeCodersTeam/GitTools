using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["feature"], "Commands for managing features")]

namespace CreativeCoders.GitTool.Cli.Commands.Features;

public static class FeaturesCommandGroup
{
    public const string Name = "feature";
}
