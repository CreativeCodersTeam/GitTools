using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

[PublicAPI]
public class CreateReleaseOptions
{
    private const string PushAllTagsLongName = "alltags";

    [OptionValue(0, IsRequired = true)]
    public string? Version { get; set; }

    [OptionParameter('a', PushAllTagsLongName)]
    public bool PushAllTags { get; set; }
}
