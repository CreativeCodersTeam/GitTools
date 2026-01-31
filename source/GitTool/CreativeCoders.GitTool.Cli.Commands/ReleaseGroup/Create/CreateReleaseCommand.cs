using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.SysConsole.Core.Abstractions;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.Create;

[UsedImplicitly]
[CliCommand([ReleaseCommandGroup.Name, "create"],
    Description = "Creates a release by creating a version tag")]
public class CreateReleaseCommand(
    ISysConsole sysConsole,
    IGitServiceProviders gitServiceProviders,
    IGitRepository gitRepository)
    : ICliCommand<CreateReleaseOptions>
{
    private readonly IGitServiceProviders _gitServiceProviders = Ensure.NotNull(gitServiceProviders);

    private readonly ISysConsole _sysConsole = Ensure.NotNull(sysConsole);

    private async Task MergeDevelopToMain(IGitRepository repository, string mainBranchName,
        CreateReleaseOptions options)
    {
        var provider = await _gitServiceProviders.GetServiceProviderAsync(repository, null);

        var createPullRequest = new GitCreatePullRequest(repository.Info.RemoteUri,
            $"Release {options.Version}", "develop", mainBranchName);

        _ = await provider.CreatePullRequestAsync(createPullRequest);
    }

    public async Task<CommandResult> ExecuteAsync(CreateReleaseOptions options)
    {
        var mainBranchName = GitBranchNames.Local.GetCanonicalName(gitRepository.Info.MainBranch);

        if (gitRepository.Branches["develop"] != null)
        {
            _sysConsole.WriteLine(
                $"Repository has a develop branch. So first a merge from develop -> {mainBranchName} must be done.");

            await MergeDevelopToMain(gitRepository, mainBranchName, options);
        }

        var tagName = $"v{options.Version}";

        _sysConsole.WriteLine($"Create tag '{tagName}'");

        gitRepository.Branches.CheckOut(mainBranchName);

        gitRepository.Pull();

        var versionTag = gitRepository.Tags.CreateTagWithMessage(tagName, mainBranchName, $"Version {options.Version}");

        if (options.PushAllTags)
        {
            _sysConsole.WriteLine("Push all tags to remote");

            gitRepository.Tags.PushAllTags();
        }
        else
        {
            _sysConsole.WriteLine($"Push tag '{versionTag.Name.Canonical}'");

            gitRepository.Tags.PushTag(versionTag);
        }

        return CommandResult.Success;
    }
}
