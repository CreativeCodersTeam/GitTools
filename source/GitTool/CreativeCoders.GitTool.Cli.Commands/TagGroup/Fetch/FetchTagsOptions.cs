using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.TagGroup.Fetch;

[UsedImplicitly]
public class FetchTagsOptions
{
    [OptionParameter('r', "remote", HelpText = "The remote to fetch tags from")]
    public string RemoteName { get; set; } = "origin";

    [OptionParameter('p', "prune", HelpText = "Prune deleted remote tags")]
    public bool Prune { get; set; } = true;
}
