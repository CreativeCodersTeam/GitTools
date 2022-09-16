using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

public class CreateReleaseCommand : IGitToolCommandWithOptions<CreateReleaseOptions>
{
    private readonly IGitServiceProviders _gitServiceProviders;

    private readonly ISysConsole _sysConsole;

    public CreateReleaseCommand(ISysConsole sysConsole,
        IGitServiceProviders gitServiceProviders)
    {
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
        _gitServiceProviders = Ensure.NotNull(gitServiceProviders, nameof(gitServiceProviders));
    }

    public async Task<int> ExecuteAsync(IGitRepository gitRepository, CreateReleaseOptions options)
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

        return 0;
    }

    private async Task MergeDevelopToMain(IGitRepository repository, string mainBranchName,
        CreateReleaseOptions options)
    {
        var provider = await _gitServiceProviders.GetServiceProviderAsync(repository, null);

        var createPullRequest = new GitCreatePullRequest(repository.Info.RemoteUri,
            $"Release {options.Version}", "develop", mainBranchName);

        var _ = await provider.CreatePullRequestAsync(createPullRequest);

        
    }
}