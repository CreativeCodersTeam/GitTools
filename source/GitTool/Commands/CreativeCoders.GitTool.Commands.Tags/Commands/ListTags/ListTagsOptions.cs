using CreativeCoders.SysConsole.Cli.Parsing;

namespace CreativeCoders.GitTool.Commands.Tags.Commands.ListTags;

public class ListTagsOptions
{
    [OptionParameter('b', "branch")]
    public string? Branch { get; set; }
}
