using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

[PublicAPI]
public class FinishFeatureOptions
{
    private const string PullRequestTitleName = "prtitle";

    [OptionValue(0, IsRequired = true)] public string FeatureName { get; set; } = null!;

    [OptionParameter('t', PullRequestTitleName, HelpText = "Title for pull request that is created")]
    public string? PullRequestTitle { get; set; }
}
