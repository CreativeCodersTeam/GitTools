using CreativeCoders.Cli.Core;
using CreativeCoders.Core;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Tool.ShowConfig;

[CliCommand(["showconfig"], Description = "Shows the current repository configuration")]
public class ShowConfigCommand(
    IAnsiConsole ansiConsole,
    IRepositoryConfigurations repositoryConfigurations,
    IGitRepositoryFactory gitRepositoryFactory) : ICliCommand
{
    private readonly IAnsiConsole _ansiConsole = Ensure.NotNull(ansiConsole);

    private readonly IRepositoryConfigurations _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);

    private readonly IGitRepositoryFactory _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory);

    public Task<CommandResult> ExecuteAsync()
    {
        using var repository = _gitRepositoryFactory.OpenRepository(Env.CurrentDirectory);

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        _ansiConsole.PrintBlock()
            .WriteLine()
            .WriteLine($"Configuration for '{repository.Info.RemoteUri}'")
            .WriteLine()
            .WriteLine($"HasDevelopBranch: {configuration.HasDevelopBranch}")
            .WriteLine($"DevelopBranch: {configuration.DevelopBranch}")
            .WriteLine($"FeatureBranchPrefix: {configuration.FeatureBranchPrefix}")
            .WriteLine($"GitServiceProviderName: {configuration.GitServiceProviderName}")
            .WriteLine($"DisableCertificateValidation: {configuration.DisableCertificateValidation}")
            .WriteLine();

        return Task.FromResult(CommandResult.Success);
    }
}
