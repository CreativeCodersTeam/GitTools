using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Features.Commands.FinishFeature
{
    [UsedImplicitly]
    public class FinishFeatureOptions
    {
        private const string PullRequestTitleName = "prtitle";

        [OptionValue(0, IsRequired = true)]
        public string FeatureName { get; [UsedImplicitly] set; } = null!;

        [OptionParameter('t', PullRequestTitleName, HelpText = "Title for pull request that is created")]
        public string? PullRequestTitle { get; set; }
    }
}
