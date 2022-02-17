using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Features.Commands.StartFeature
{
    [UsedImplicitly]
    public class StartFeatureOptions
    {
        private const string PushAfterCreateName = "pushaftercreate";

        [OptionValue(0, IsRequired = true)]
        public string FeatureName { get; [UsedImplicitly] set; } = null!;

        [OptionParameter('p', PushAfterCreateName)]
        public bool PushAfterCreate { get; set; }
    }
}
