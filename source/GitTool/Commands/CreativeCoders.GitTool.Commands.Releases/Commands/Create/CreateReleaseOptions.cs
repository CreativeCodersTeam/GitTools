using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

public class CreateReleaseOptions
{
    [OptionValue(0, IsRequired = true)]
    public string Version { get; set; }

    [OptionParameter('a', "alltags")]
    public bool PushAllTags { get; set; }
}