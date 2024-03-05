using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.NukeBuild.BuildActions;
using CreativeCoders.NukeBuild.Components;
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
    InvokedTargets = [NukeTargets.DeployNuGet],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("integration-win", GitHubActionsImage.WindowsLatest,
    OnPushBranches = ["feature/**"],
    InvokedTargets =
    [
        NukeTargets.Rebuild, NukeTargets.CodeCoverage, "CreateWin64Setup",
        nameof(ICreateGithubReleaseTarget.CreateGithubRelease)
    ],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("pull-request", GitHubActionsImage.UbuntuLatest, GitHubActionsImage.WindowsLatest,
    OnPullRequestBranches = ["main"],
    InvokedTargets = [NukeTargets.Rebuild, NukeTargets.CodeCoverage, NukeTargets.Pack],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("main", GitHubActionsImage.UbuntuLatest,
    OnPushBranches = ["main"],
    InvokedTargets = [NukeTargets.DeployNuGet],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions("main-win", GitHubActionsImage.WindowsLatest,
    OnPushBranches = ["main"],
    InvokedTargets = [NukeTargets.Rebuild, NukeTargets.CodeCoverage, "CreateWin64Setup"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions(ReleaseWorkflow, GitHubActionsImage.UbuntuLatest,
    OnPushTags = ["v**"],
    InvokedTargets = [NukeTargets.DeployNuGet],
    ImportSecrets = ["NUGET_ORG_TOKEN"],
    EnableGitHubToken = true,
    PublishArtifacts = true,
    FetchDepth = 0
)]
[GitHubActions(ReleaseWorkflow + "-win", GitHubActionsImage.WindowsLatest,
    OnPushTags = ["v**"],
    InvokedTargets = [NukeTargets.Rebuild, NukeTargets.CodeCoverage, "CreateWin64Setup"],
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
    IDeployNuGetTarget, IPublishTarget, ICreateGithubReleaseTarget
{
    public const string ReleaseWorkflow = "release";

    readonly GitHubActions GitHubActions = GitHubActions.Instance;

    [Parameter(Name = "GITHUB_TOKEN")] string DevNuGetApiKey;

    [Parameter(Name = "NUGET_ORG_TOKEN")] string NuGetOrgApiKey;

    public Build()
    {
        this.DisableAllTelemetry();
    }

    Target CreateWin64Setup => d => d
        .OnlyWhenDynamic(() => GitHubActions.IsRunnerOs(GitHubActionsRunnerOs.Windows))
        .Before<ICreateGithubReleaseTarget>()
        .DependsOn<IPublishTarget>()
        .Produces(this.GetArtifactsDirectory() / "setups" / "*.*")
        .Executes(() => InnoSetupTasks
            .InnoSetup(x => x
                .SetScriptFile(RootDirectory / "setup" / "GitTool.iss")
                .AddKeyValueDefinition(
                    "CiAppVersion", GetVersion())));

    IList<AbsolutePath> ICleanSettings.DirectoriesToClean =>
        this.As<ICleanSettings>().DefaultDirectoriesToClean
            .AddRange(this.As<ITestSettings>().TestBaseDirectory);

    public IEnumerable<Project> TestProjects => GetTestProjects();

    string ICreateGithubReleaseSettings.ReleaseName =>
        $"v{GetVersion()}";

    string ICreateGithubReleaseSettings.ReleaseBody =>
        $"Version {GetVersion()}";

    string ICreateGithubReleaseSettings.ReleaseVersion =>
        this.GetVersion();

    IEnumerable<GithubReleaseAsset> ICreateGithubReleaseSettings.ReleaseAssets
    {
        get
        {
            var fileName = GetSetupFileName();

            if (fileName == string.Empty)
            {
                return [];
            }

            return [new GithubReleaseAsset(fileName)];
        }
    }

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

            return
            [
                new PublishingItem(
                    sourceDir.SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                    "CreativeCoders.GitTool.Cli.csproj", this.GetArtifactsDirectory() / "GitTool.Cli"),
                new PublishingItem(sourceDir.SourceDirectory / "GitTool" / "CreativeCoders.GitTool.Cli" /
                                   "CreativeCoders.GitTool.Cli.csproj",
                    this.GetArtifactsDirectory() / "GitTool.Cli.Win64")
                {
                    Runtime = DotNetRuntime.WinX64,
                    SelfContained = true
                }
            ];
        }
    }

    bool IPushNuGetSettings.SkipPush => GitHubActions?.IsPullRequest == true;

    string IPushNuGetSettings.NuGetFeedUrl =>
        GitHubActions?.Workflow.StartsWith(ReleaseWorkflow) == true
            ? "nuget.org"
            : "https://nuget.pkg.github.com/CreativeCodersTeam/index.json";

    string IPushNuGetSettings.NuGetApiKey =>
        GitHubActions?.Workflow.StartsWith(ReleaseWorkflow) == true
            ? NuGetOrgApiKey
            : DevNuGetApiKey;

    string IPackSettings.PackageProjectUrl => "https://github.com/CreativeCodersTeam/GitTools";

    string IPackSettings.PackageLicenseExpression => PackageLicenseExpressions.ApacheLicense20;

    string IPackSettings.Copyright => $"{DateTime.Now.Year} CreativeCoders";

    string GetVersion() => this.GetGitVersion()?.NuGetVersionV2 ?? "0.1-local";

    string GetSetupFileName()
    {
        var setupsOutputPath = this.As<IArtifactsSettings>().ArtifactsDirectory / "setups";

        var setupFiles = setupsOutputPath.GlobFiles("gittool_setup_*.exe");

        return setupFiles.Count switch
        {
            > 1 => throw new InvalidOperationException("Multiple setup files found"),
            0 => throw new InvalidOperationException("No setup files found"),
            _ => setupFiles.Single()
        };
    }

    Project[] GetTestProjects()
    {
        return this.TryAs<ISolutionParameter>(out var solutionParameter)
            ? solutionParameter.Solution.GetAllProjects("*")
                .Where(x => ((string)x.Path)?.StartsWith(RootDirectory / "tests") ?? false).ToArray()
            : Array.Empty<Project>();
    }

    public static int Main() => Execute<Build>(x => ((ICodeCoverageTarget)x).CodeCoverage);
}
