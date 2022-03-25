using System;
using System.IO.Compression;
using CreativeCoders.NukeBuild;
using CreativeCoders.NukeBuild.BuildActions;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

[CheckBuildProjectConfigurations]
class Build : NukeBuild, IBuildInfo
{
    public static int Main () => Execute<Build>(x => x.RunBuild);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "source";

    AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";

    AbsolutePath TestBaseDirectory => RootDirectory / ".tests";

    AbsolutePath TestResultsDirectory => TestBaseDirectory / "results";

    AbsolutePath TestProjectsBasePath => RootDirectory / "tests";

    AbsolutePath CoverageDirectory => TestBaseDirectory / "coverage";

    const string PackageProjectUrl = "https://github.com/CreativeCodersTeam/GitTools";

    [Parameter] string NuGetSource;

    [Parameter] string NuGetApiKey;

    Target Clean => _ => _
        .Before(Restore)
        .UseBuildAction<CleanBuildAction>(this,
            x => x
                .AddDirectoryForClean(ArtifactsDirectory)
                .AddDirectoryForClean(TestBaseDirectory));

    Target Restore => _ => _
        .Before(Compile)
        .UseBuildAction<RestoreBuildAction>(this);

    Target Compile => _ => _
        .After(Clean)
        .UseBuildAction<DotNetCompileBuildAction>(this);

    Target Test => _ => _
        .After(Compile)
        .UseBuildAction<DotNetTestAction>(this,
            x => x
                .SetTestProjectsBaseDirectory(TestProjectsBasePath)
                .SetProjectsPattern("**/*.csproj")
                .SetResultsDirectory(TestResultsDirectory)
                .UseLogger("trx")
                .SetResultFileExt("trx")
                .EnableCoverage()
                .SetCoverageDirectory(CoverageDirectory));

    Target CoverageReport => _ => _
        .After(Test)
        .UseBuildAction<CoverageReportAction>(this,
            x => x
                .SetReports(TestBaseDirectory / "coverage" / "**" / "*.xml")
                .SetTargetDirectory(TestBaseDirectory / "coverage_report"));

    Target Pack => _ => _
        .After(Compile)
        .UseBuildAction<PackBuildAction>(this,
            x => x
                .SetPackageLicenseExpression(PackageLicenseExpressions.ApacheLicense20)
                .SetPackageProjectUrl(PackageProjectUrl)
                .SetCopyright($"{DateTime.Now.Year} CreativeCoders")
                .SetEnableNoBuild(false));

    Target Publish => _ => _
        .UseBuildAction<DotNetPublishBuildAction>(this,
            x => x
                .SetProject(SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                            "CreativeCoders.GitTool.Cli.csproj")
                .SetOutput(ArtifactsDirectory / "GitTool.Cli"));

    Target PushToNuGet => _ => _
        .Requires(() => NuGetApiKey)
        .UseBuildAction<PushBuildAction>(this,
            x => x
                .SetSource(NuGetSource)
                .SetApiKey(NuGetApiKey));

    Target CreateCliZip => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            ZipFile.CreateFromDirectory(ArtifactsDirectory / "GitTool.Cli", ArtifactsDirectory / "GitTool.Cli.zip");
        });

    Target RunBuild => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(Compile);

    Target RunTest => _ => _
        .DependsOn(RunBuild)
        .DependsOn(Test)
        .DependsOn(CoverageReport);

    Target CreateNuGetPackages => _ => _
        .DependsOn(RunTest)
        .DependsOn(Pack);

    string IBuildInfo.Configuration => Configuration;

    Solution IBuildInfo.Solution => Solution;

    GitRepository IBuildInfo.GitRepository => GitRepository;

    IVersionInfo IBuildInfo.VersionInfo => new GitVersionWrapper(GitVersion, "0.0.0", 1);

    AbsolutePath IBuildInfo.SourceDirectory => SourceDirectory;

    AbsolutePath IBuildInfo.ArtifactsDirectory => ArtifactsDirectory;
}
