using CreativeCoders.Cli.Core;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Tool;

[CliCommand(["showconfig"], Description = "Shows the current repository configuration")]
public class ShowConfigCommand(
    IAnsiConsole console,
    IRepositoryConfigurations repositoryConfigurations,
    IGitRepositoryFactory gitRepositoryFactory) : ICliCommand
{
    public Task<CommandResult> ExecuteAsync()
    {
        using var repository = gitRepositoryFactory.OpenRepository(Env.CurrentDirectory);

        var configuration = repositoryConfigurations.GetConfiguration(repository);

        console.PrintBlock()
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
