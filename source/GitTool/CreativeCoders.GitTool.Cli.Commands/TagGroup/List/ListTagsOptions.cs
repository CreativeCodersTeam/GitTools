using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.List;

[UsedImplicitly]
public class ListTagsOptions
{
    [OptionParameter('x', "extended", HelpText = "Show extended information")]
    public bool ShowExtendedInformation { get; set; }
}
