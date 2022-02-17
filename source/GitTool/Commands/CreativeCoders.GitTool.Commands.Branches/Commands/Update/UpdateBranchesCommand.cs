using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Update
{
    public class UpdateBranchesCommand : IUpdateBranchesCommand
    {
        private readonly IRepositoryConfigurations _repositoryConfigurations;

        private readonly ISysConsole _sysConsole;

        private readonly IGitRepositoryFactory _gitRepositoryFactory;

        public UpdateBranchesCommand(IGitRepositoryFactory gitRepositoryFactory, ISysConsole sysConsole,
            IRepositoryConfigurations repositoryConfigurations)
        {
            _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations, nameof(repositoryConfigurations));
            _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
            _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        }

        public Task<int> ExecuteAsync(UpdateBranchesOptions options)
        {
            _sysConsole
                .WriteLine()
                .WriteLine("Update permanent local branches")
                .WriteLine();

            using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

            var configuration = _repositoryConfigurations.GetConfiguration(repository);

            var currentBranch = repository.Head;

            var updateBranchNames = new System.Collections.Generic.List<string>
            {
                "production",
                GitBranchNames.Local.GetFriendlyName(repository.Info.MainBranch)
            };

            if (configuration.HasDevelopBranch)
            {
                updateBranchNames.Add(configuration.DevelopBranch);
            }

            updateBranchNames
                .ForEach(branchName =>
                {
                    if (repository.Branches[branchName] == null)
                    {
                        return;
                    }

                    _sysConsole.WriteLine($"Update local branch '{branchName}'");

                    repository.CheckOut(branchName);

                    repository.Pull();
                });

            if (!repository.Head.Equals(currentBranch))
            {
                _sysConsole.WriteLine($"Switch back to working branch '{currentBranch.Name.Friendly}'");

                repository.CheckOut(currentBranch.Name.Friendly);
            }

            return Task.FromResult(0);
        }
    }
}
