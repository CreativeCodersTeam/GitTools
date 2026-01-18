using Cake.Common.Build;
using Cake.Core;
using Cake.Core.IO;
using CreativeCoders.CakeBuild;
using CreativeCoders.CakeBuild.Tasks.Defaults;
using CreativeCoders.CakeBuild.Tasks.Templates.Settings;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using JetBrains.Annotations;

namespace Build;

[UsedImplicitly]
public class BuildContext(ICakeContext context)
    : CakeBuildContext(context), IDefaultTaskSettings, ICreateDistPackagesTaskSettings
{
    public IList<DirectoryPath> DirectoriesToClean => this.CastAs<ICleanTaskSettings>()
        .GetDefaultDirectoriesToClean().AddRange(RootDir.Combine(".tests"));


    public string Copyright => $"{DateTime.Now.Year} CreativeCoders";

    public string PackageProjectUrl => "https://github.com/CreativeCodersTeam/GitTools";

    public string PackageLicenseExpression => PackageLicenseExpressions.ApacheLicense20;

    public string NuGetFeedUrl => this.GitHubActions().Environment.Workflow.Workflow == "release"
        ? "nuget.org"
        : "https://nuget.pkg.github.com/CreativeCodersTeam/index.json";

    public bool SkipPush => this.BuildSystem().IsPullRequest ||
                            this.BuildSystem().IsLocalBuild ||
                            this.GitHubActions().Environment.Runner.OS != "Linux";

    public DirectoryPath PublishOutputDir => ArtifactsDir.Combine("published");

    public IEnumerable<PublishingItem> PublishingItems =>
    [
        new PublishingItem(
            RootDir
                .Combine("source/GitTool/CreativeCoders.GitTool.Cli")
                .CombineWithFilePath("CreativeCoders.GitTool.Cli.csproj"),
            PublishOutputDir.Combine("cli"))
    ];

    public IEnumerable<DistPackage> DistPackages =>
    [
        new DistPackage("GitTool.Cli", PublishOutputDir.Combine("cli"))
    ];
}
