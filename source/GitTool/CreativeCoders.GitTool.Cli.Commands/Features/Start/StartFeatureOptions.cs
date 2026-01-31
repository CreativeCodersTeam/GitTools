using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.Features.Start;

[PublicAPI]
public class StartFeatureOptions
{
    private const string PushAfterCreateName = "pushaftercreate";

    [OptionValue(0, IsRequired = true)] public string FeatureName { get; set; } = null!;

    [OptionParameter('p', PushAfterCreateName)]
    public bool PushAfterCreate { get; set; }
}
