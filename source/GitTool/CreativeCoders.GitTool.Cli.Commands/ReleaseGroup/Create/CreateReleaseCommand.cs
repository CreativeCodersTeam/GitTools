using CreativeCoders.Cli.Core;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Versioning;
using CreativeCoders.SysConsole.Core;
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

        if (options is { VersionIncrement: not null, ConfirmAutoIncrementVersion: true })
        {
            _ansiConsole.MarkupLine($"Version will be incremented to '{version}'".ToInfoMarkup());

            if (!await _ansiConsole.PromptAsync(new TextPrompt<bool>("Do you want to continue?").DefaultValue(true))
                    .ConfigureAwait(false))
            {
                throw new CliCommandAbortException("Release creation aborted.", ReturnCodes.ReleaseCreationAborted)
                {
                    IsError = false
                };
            }
        }

        var tagName = $"v{version}";

        _ansiConsole.WriteLine($"Creating tag '{tagName}'");

        _gitRepository.Branches.CheckOut(mainBranchName);

        _gitRepository.Pull();

        var versionTag =
            _gitRepository.Tags.CreateTagWithMessage(tagName, $"Version {version}", mainBranchName);

        _ansiConsole.MarkupLine($"Tag '{tagName}' created".ToSuccessMarkup());

        if (options.PushAllTags)
        {
            _ansiConsole.WriteLine("Pushing all tags to remote");

            _gitRepository.Tags.PushAllTags();

            _ansiConsole.MarkupLine("All tags pushed successfully".ToSuccessMarkup());
        }
        else
        {
            _ansiConsole.WriteLine($"Push tag '{versionTag.Name.Canonical}'");

            _gitRepository.Tags.PushTag(versionTag);

            _ansiConsole.MarkupLine($"Tag '{versionTag.Name.Canonical}' pushed successfully".ToSuccessMarkup());
        }

        return CommandResult.Success;
    }

    private string CreateVersion(CreateReleaseOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Version))
        {
            return new VersionBuilder(options.Version).Build();
        }

        _gitRepository.FetchAllTags("origin");

        var greatestVersion = _gitRepository
            .GetVersionTags()
            .OrderByDescending(x => x.Version, new VersionComparer())
            .FirstOrDefault();

        var versionBuilder =
            new VersionBuilder(string.IsNullOrWhiteSpace(greatestVersion?.Version)
                ? DefaultBaseVersionForIncrement
                : greatestVersion.Version);

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

    private async Task MergeDevelopToMain(IGitRepository repository, string mainBranchName,
        CreateReleaseOptions options)
    {
        var provider = await _gitServiceProviders.GetServiceProviderAsync(repository, null);

        var createPullRequest = new GitCreatePullRequest(repository.Info.RemoteUri,
            $"Release {options.Version}", "develop", mainBranchName);

        _ = await provider.CreatePullRequestAsync(createPullRequest);
    }
}
