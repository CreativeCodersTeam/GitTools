using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

public class CreateReleaseOptions
{
    [OptionParameter('v', "version", IsRequired = true)]
    public string Version { get; set; }
}