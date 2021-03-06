using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.Create;

public class CreateReleaseCommand : ICreateReleaseCommand
{
    private readonly IGitServiceProviders _gitServiceProviders;

    private readonly ISysConsole _sysConsole;

    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    public CreateReleaseCommand(IGitRepositoryFactory gitRepositoryFactory, ISysConsole sysConsole,
        IGitServiceProviders gitServiceProviders)
    {
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _gitServiceProviders = Ensure.NotNull(gitServiceProviders, nameof(gitServiceProviders));
    }

    public async Task<int> ExecuteAsync(CreateReleaseOptions options)
    {
        using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var mainBranchName = GitBranchNames.Local.GetCanonicalName(repository.Info.MainBranch);

        if (repository.Branches["develop"] != null)
        {
            _sysConsole.WriteLine(
                $"Repository has a develop branch. So first a merge from develop -> {mainBranchName} must be done.");

            await MergeDevelopToMain(repository, mainBranchName, options);
        }

        var tagName = $"v{options.Version}";

        _sysConsole.WriteLine($"Create tag '{tagName}'");

        repository.CheckOut(mainBranchName);

        repository.Pull();

        var versionTag = repository.CreateTagWithMessage(tagName, mainBranchName, $"Version {options.Version}");

        if (options.PushAllTags)
        {
            _sysConsole.WriteLine("Push all tags to remote");

            repository.PushAllTags();
        }
        else
        {
            _sysConsole.WriteLine($"Push tag '{versionTag.Name.Canonical}'");

            repository.PushTag(versionTag);
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