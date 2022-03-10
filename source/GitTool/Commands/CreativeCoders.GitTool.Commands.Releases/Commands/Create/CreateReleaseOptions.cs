using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

public class CreateReleaseOptions
{
    private const string PushAllTagsLongName = "alltags";

    [OptionValue(0, IsRequired = true)]
    public string Version { get; set; }

    [OptionParameter('a', PushAllTagsLongName)]
    public bool PushAllTags { get; set; }
}