using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Versioning;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.Create;

[UsedImplicitly]
[CliCommand([ReleaseCommandGroup.Name, "create"],
    Description = "Creates a release by creating a version tag")]
public class CreateReleaseCommand(
    IAnsiConsole ansiConsole,
    IGitServiceProviders gitServiceProviders,
    IGitRepository gitRepository)
    : ICliCommand<CreateReleaseOptions>
{
    private const string DefaultBaseVersionForIncrement = "0.0.0";

    private readonly IGitServiceProviders _gitServiceProviders = Ensure.NotNull(gitServiceProviders);

    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IGitRepository _gitRepository = Ensure.NotNull(gitRepository);

    public async Task<CommandResult> ExecuteAsync(CreateReleaseOptions options)
    {
        var mainBranchName = GitBranchNames.Local.GetCanonicalName(_gitRepository.Info.MainBranch);

        if (_gitRepository.Branches["develop"] != null)
        {
            _ansiConsole.WriteLine(
                $"Repository has a develop branch. So first a merge from develop -> {mainBranchName} must be done.");

            await MergeDevelopToMain(_gitRepository, mainBranchName, options);
        }

        var version = CreateVersion(options);

        var tagName = $"v{version}";

        _ansiConsole.WriteLine($"Create tag '{tagName}'");

        _gitRepository.Branches.CheckOut(mainBranchName);

        _gitRepository.Pull();

        var versionTag =
            _gitRepository.Tags.CreateTagWithMessage(tagName, $"Version {version}", mainBranchName);

        if (options.PushAllTags)
        {
            _ansiConsole.WriteLine("Push all tags to remote");

            _gitRepository.Tags.PushAllTags();
        }
        else
        {
            _ansiConsole.WriteLine($"Push tag '{versionTag.Name.Canonical}'");

            _gitRepository.Tags.PushTag(versionTag);
        }

        return CommandResult.Success;
    }

    private string CreateVersion(CreateReleaseOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Version))
        {
            return new VersionBuilder(options.Version).Build();
        }

        var greatestVersion = GetVersionTags().OrderByDescending(x => x).FirstOrDefault();

        var versionBuilder =
            new VersionBuilder(string.IsNullOrWhiteSpace(greatestVersion)
                ? DefaultBaseVersionForIncrement
                : greatestVersion);

        switch (options.VersionIncrement!)
        {
            case VersionAutoIncrement.Major: versionBuilder.IncrementMajor(); break;
            case VersionAutoIncrement.Minor: versionBuilder.IncrementMinor(); break;
            case VersionAutoIncrement.Patch: versionBuilder.IncrementPatch(); break;
            default:
                throw new InvalidOperationException("Unknown VersionAutoIncrement");
        }

        return versionBuilder.Build();
    }

    private IEnumerable<string> GetVersionTags()
    {
        _gitRepository.FetchAllTags("origin");

        foreach (var tag in _gitRepository.Tags)
        {
            if (VersionUtils.IsValidVersion(tag.Name.Friendly, out var version))
            {
                yield return version;
            }
        }
    }

    private async Task MergeDevelopToMain(IGitRepository repository, string mainBranchName,
        CreateReleaseOptions options)
    {
        var provider = await _gitServiceProviders.GetServiceProviderAsync(repository, null);

        var createPullRequest = new GitCreatePullRequest(repository.Info.RemoteUri,
            $"Release {options.Version}", "develop", mainBranchName);

        _ = await provider.CreatePullRequestAsync(createPullRequest);
    }
}
