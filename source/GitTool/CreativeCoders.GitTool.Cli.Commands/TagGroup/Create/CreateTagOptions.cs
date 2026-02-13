using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Create;

[UsedImplicitly]
public class CreateTagOptions
{
    [OptionValue(0, IsRequired = true)] public string TagName { get; set; } = string.Empty;

    [OptionParameter('p', "push", HelpText = "Push tag after creation")]
    public bool PushAfterCreate { get; set; }

    [OptionParameter('m', "message", HelpText = "Message for tag")]
    public string? Message { get; set; }

    [OptionParameter('o', "objectish", HelpText = "Object to tag")]
    public string? Objectish { get; set; }
}
