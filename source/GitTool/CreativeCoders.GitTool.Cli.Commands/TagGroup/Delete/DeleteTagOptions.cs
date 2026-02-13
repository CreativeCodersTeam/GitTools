using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Delete;

[UsedImplicitly]
public class DeleteTagOptions
{
    [OptionValue(0, HelpText = "The name of the tag to delete")]
    public string TagName { get; set; } = string.Empty;

    [OptionParameter('r', "remote", HelpText = "Delete tag on remote")]
    public bool DeleteOnRemote { get; set; }
}
