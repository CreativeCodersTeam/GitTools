using CreativeCoders.Cli.Core;

[assembly: CliCommandGroup(["feature"], "Commands for managing features")]

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup;

public static class FeatureCommandGroup
{
    public const string Name = "feature";
}
