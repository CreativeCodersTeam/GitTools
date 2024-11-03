using CreativeCoders.Core;
using CreativeCoders.Core.Reflection;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Configurations;

namespace CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

public class GitToolCommandExecutor : IGitToolCommandExecutor
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    private readonly IServiceProvider _serviceProvider;

    public GitToolCommandExecutor(IServiceProvider serviceProvider, IGitRepositoryFactory gitRepositoryFactory,
        IRepositoryConfigurations repositoryConfigurations)
    {
        _serviceProvider = Ensure.NotNull(serviceProvider);
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory);
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);
    }

    public async Task<int> ExecuteAsync<TCommand>()
        where TCommand : IGitToolCommand
    {
        var command = typeof(TCommand).CreateInstance<IGitToolCommand>(_serviceProvider);

        if (command == null)
        {
            throw new InvalidOperationException("Command object cannot be created");
        }

        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        if (configuration.DisableCertificateValidation)
        {
            gitRepository.CertificateCheck = (_, args) =>
            {
                args.IsValid = true;
                return true;
            };
        }

        return await command.ExecuteAsync(gitRepository).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync<TCommand, TCommandOptions>(TCommandOptions options)
        where TCommand : IGitToolCommandWithOptions<TCommandOptions>
    {
        var command = typeof(TCommand).CreateInstance<IGitToolCommandWithOptions<TCommandOptions>>(_serviceProvider);

        if (command == null)
        {
            throw new InvalidOperationException("Command object cannot be created");
        }

        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var configuration = _repositoryConfigurations.GetConfiguration(gitRepository);

        if (configuration.DisableCertificateValidation)
        {
            gitRepository.CertificateCheck = (_, args) =>
            {
                args.IsValid = true;
                return true;
            };
        }

        return await command.ExecuteAsync(gitRepository, options).ConfigureAwait(false);
    }
}
