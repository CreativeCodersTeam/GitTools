using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.NukeBuild.BuildActions;
using CreativeCoders.NukeBuild.Components.Parameters;
using CreativeCoders.NukeBuild.Components.Targets;
using CreativeCoders.NukeBuild.Components.Targets.Settings;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.InnoSetup;

#pragma warning disable S1144 // remove unused private members
#pragma warning disable S3903 // move class to namespace

[PublicAPI]
[UnsetVisualStudioEnvironmentVariables]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
[GitHubActions("integration", GitHubActionsImage.UbuntuLatest,
    OnPushBranches = ["feature/**"],
    InvokedTargets = ["deploynuget"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("integration-win", GitHubActionsImage.WindowsLatest,
    OnPushBranches = ["feature/**"],
    InvokedTargets = ["Rebuild", "CodeCoverage", "CreateWin64Setup"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("pull-request", GitHubActionsImage.UbuntuLatest, GitHubActionsImage.WindowsLatest,
    OnPullRequestBranches = ["main"],
    InvokedTargets = ["rebuild", "codecoverage", "pack"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("main", GitHubActionsImage.UbuntuLatest, GitHubActionsImage.WindowsLatest,
    OnPushBranches = ["main"],
    InvokedTargets = ["deploynuget", "CreateWin64Setup"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions(ReleaseWorkflow, GitHubActionsImage.UbuntuLatest, GitHubActionsImage.WindowsLatest,
    OnPushTags = ["v**"],
    InvokedTargets = ["deploynuget", "CreateWin64Setup"],
    ImportSecrets = ["NUGET_ORG_TOKEN"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
class Build : NukeBuild, IGitRepositoryParameter,
    IConfigurationParameter,
    IGitVersionParameter,
    ISourceDirectoryParameter,
    IArtifactsSettings,
    ICleanTarget, IBuildTarget, IRestoreTarget, ICodeCoverageTarget, IPushNuGetTarget, IRebuildTarget,
    IDeployNuGetTarget, IPublishTarget
{
    public const string ReleaseWorkflow = "release";

    readonly GitHubActions GitHubActions = GitHubActions.Instance;

    [Parameter(Name = "GITHUB_TOKEN")] string DevNuGetApiKey;

    [Parameter(Name = "NUGET_ORG_TOKEN")] string NuGetOrgApiKey;

    public Build()
    {
        Environment.SetEnvironmentVariable("DOTNET_CLI_TELEMETRY_OPTOUT", "1");
        Environment.SetEnvironmentVariable("NUKE_TELEMETRY_OPTOUT", "1");
    }

    Target CreateWin64Setup => d => d
        .OnlyWhenDynamic(() =>
            Environment.GetEnvironmentVariable("RUNNER_OS")?.Equals("Windows", StringComparison.Ordinal) == true)
        .DependsOn<IPublishTarget>()
        .Produces(this.As<IArtifactsSettings>().ArtifactsDirectory / "setups" / "*.*")
        .Executes(() => InnoSetupTasks
            .InnoSetup(x => x
                .SetScriptFile(RootDirectory / "setup" / "GitTool.iss")
                .AddKeyValueDefinition(
                    "CiAppVersion", this.As<IGitVersionParameter>().GitVersion?.NuGetVersionV2)));

    IList<AbsolutePath> ICleanSettings.DirectoriesToClean =>
        this.As<ICleanSettings>().DefaultDirectoriesToClean
            .AddRange(this.As<ITestSettings>().TestBaseDirectory);

    public IEnumerable<Project> TestProjects => GetTestProjects();

    DotNetPublishSettings IPublishTarget.ConfigurePublishSettings(DotNetPublishSettings publishSettings)
    {
        return this.As<IPublishTarget>().ConfigureDefaultPublishSettings(publishSettings)
            .SetNoBuild(false);
    }

    IEnumerable<PublishingItem> IPublishSettings.PublishingItems
    {
        get
        {
            var sourceDir = this.As<ISourceDirectoryParameter>();
            var artifactsDir = this.As<IArtifactsSettings>();

            return
            [
                new PublishingItem(
                    sourceDir.SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                    "CreativeCoders.GitTool.Cli.csproj", artifactsDir.ArtifactsDirectory / "GitTool.Cli"),
                new PublishingItem(sourceDir.SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                                   "CreativeCoders.GitTool.Cli.csproj",
                    artifactsDir.ArtifactsDirectory / "GitTool.Cli.Win64")
                {
                    Runtime = DotNetRuntime.WinX64,
                    SelfContained = true
                }
            ];
        }
    }

    bool IPushNuGetSettings.SkipPush => GitHubActions?.IsPullRequest == true;

    string IPushNuGetSettings.NuGetFeedUrl =>
        GitHubActions?.Workflow == ReleaseWorkflow
            ? "nuget.org"
            : "https://nuget.pkg.github.com/CreativeCodersTeam/index.json";

    string IPushNuGetSettings.NuGetApiKey =>
        GitHubActions?.Workflow == ReleaseWorkflow
            ? NuGetOrgApiKey
            : DevNuGetApiKey;

    string IPackSettings.PackageProjectUrl => "https://github.com/CreativeCodersTeam/GitTools";

    string IPackSettings.PackageLicenseExpression => PackageLicenseExpressions.ApacheLicense20;

    string IPackSettings.Copyright => $"{DateTime.Now.Year} CreativeCoders";

    Project[] GetTestProjects()
    {
        return this.TryAs<ISolutionParameter>(out var solutionParameter)
            ? solutionParameter.Solution.GetAllProjects("*")
                .Where(x => ((string)x.Path)?.StartsWith(RootDirectory / "tests") ?? false).ToArray()
            : Array.Empty<Project>();
    }

    public static int Main() => Execute<Build>(x => ((ICodeCoverageTarget)x).CodeCoverage);
}
