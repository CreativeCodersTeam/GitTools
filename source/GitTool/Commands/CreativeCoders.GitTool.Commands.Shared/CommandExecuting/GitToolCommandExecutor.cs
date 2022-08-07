using CreativeCoders.Core;
using CreativeCoders.Core.Reflection;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

public class GitToolCommandExecutor : IGitToolCommandExecutor
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    public GitToolCommandExecutor(IServiceProvider serviceProvider, IGitRepositoryFactory gitRepositoryFactory)
    {
        _serviceProvider = Ensure.NotNull(serviceProvider, nameof(serviceProvider));
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
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

        return await command.ExecuteAsync(gitRepository, options).ConfigureAwait(false);
    }
}
