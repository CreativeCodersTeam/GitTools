using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.ListVersions;

[UsedImplicitly]
public class ListVersionsOptions
{
    [OptionParameter('d', "descending", HelpText = "Sorts versions descending")]
    public bool SortDescending { get; set; }
}
