using System;
using System.Diagnostics.CodeAnalysis;
using CreativeCoders.NukeBuild;
using CreativeCoders.NukeBuild.BuildActions;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.InnoSetup;

#pragma warning disable S1144 // remove unused private members
#pragma warning disable S3903 // move class to namespace

[PublicAPI]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
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

    string PackageProjectUrl => "https://github.com/CreativeCodersTeam/GitTools";

    [Parameter] string NuGetSource;

    [Parameter] string NuGetApiKey;

    Target Clean => d => d
        .Before(Restore)
        .UseBuildAction<CleanBuildAction>(this,
            x => x
                .AddDirectoryForClean(ArtifactsDirectory)
                .AddDirectoryForClean(TestBaseDirectory));

    Target Restore => d => d
        .Before(Compile)
        .UseBuildAction<RestoreBuildAction>(this);

    Target Compile => d => d
        .After(Clean)
        .UseBuildAction<DotNetCompileBuildAction>(this);

    Target Test => d => d
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

    Target CoverageReport => d => d
        .After(Test)
        .UseBuildAction<CoverageReportAction>(this,
            x => x
                .SetReports(TestBaseDirectory / "coverage" / "**" / "*.xml")
                .SetTargetDirectory(TestBaseDirectory / "coverage_report"));

    Target Pack => d => d
        .After(Compile)
        .UseBuildAction<PackBuildAction>(this,
            x => x
                .SetPackageLicenseExpression(PackageLicenseExpressions.ApacheLicense20)
                .SetPackageProjectUrl(PackageProjectUrl)
                .SetCopyright($"{DateTime.Now.Year} CreativeCoders")
                .SetEnableNoBuild(false));

    Target Publish => d => d
        .UseBuildAction<DotNetPublishBuildAction>(this,
            x => x
                .SetProject(SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                            "CreativeCoders.GitTool.Cli.csproj")
                .SetOutput(ArtifactsDirectory / "GitTool.Cli"));

    Target PublishCliWin64 => d => d
        .DependsOn(Clean)
        .DependsOn(Restore)
        .UseBuildAction<DotNetPublishBuildAction>(this,
            x => x
                .SetProject(SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                            "CreativeCoders.GitTool.Cli.csproj")
                .SetOutput(ArtifactsDirectory / "GitTool.Cli.Win64")
                .SetRuntime(DotNetRuntime.WinX64));

    Target PushToNuGet => d => d
        .Requires(() => NuGetApiKey)
        .UseBuildAction<PushBuildAction>(this,
            x => x
                .SetSource(NuGetSource)
                .SetApiKey(NuGetApiKey));

    Target CreateWin64Setup => d => d
        .DependsOn(PublishCliWin64)
        .Executes(() => InnoSetupTasks
            .InnoSetup(x => x
                .SetScriptFile(RootDirectory / "setup" / "GitTool.iss")
                .AddKeyValueDefinition("CiAppVersion", GitVersion.NuGetVersionV2)));

    Target RunBuild => d => d
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(Compile);

    Target RunTest => d => d
        .DependsOn(RunBuild)
        .DependsOn(Test)
        .DependsOn(CoverageReport);

    Target CreateNuGetPackages => d => d
        .DependsOn(RunTest)
        .DependsOn(Pack);

    string IBuildInfo.Configuration => Configuration;

    Solution IBuildInfo.Solution => Solution;

    GitRepository IBuildInfo.GitRepository => GitRepository;

    IVersionInfo IBuildInfo.VersionInfo => new GitVersionWrapper(GitVersion, "0.0.0", 1);

    AbsolutePath IBuildInfo.SourceDirectory => SourceDirectory;

    AbsolutePath IBuildInfo.ArtifactsDirectory => ArtifactsDirectory;
}
