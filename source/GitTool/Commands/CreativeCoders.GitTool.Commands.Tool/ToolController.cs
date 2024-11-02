using System;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using CreativeCoders.SysConsole.Cli.Actions.Help;
using CreativeCoders.SysConsole.Core.Abstractions;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Tool;

[PublicAPI]
[CliController]
public class ToolController
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IGitServiceProviders _gitServiceProviders;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    private readonly ISysConsole _sysConsole;

    public ToolController(ISysConsole sysConsole, IRepositoryConfigurations repositoryConfigurations,
        IGitRepositoryFactory gitRepositoryFactory, IGitServiceProviders gitServiceProviders)
    {
        _gitServiceProviders = Ensure.NotNull(gitServiceProviders);
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory);
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);
        _sysConsole = Ensure.NotNull(sysConsole);
    }

    [CliAction("setup")]
    public async Task Setup()
    {
        using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        var developBranch = _sysConsole
            .WriteLine()
            .WriteLine("Development branch (empty = no development branch)")
            .Write($"(Current value = '{configuration.DevelopBranch}'): ")
            .ReadLine();

        configuration.DevelopBranch = developBranch ?? string.Empty;
        configuration.HasDevelopBranch = !string.IsNullOrEmpty(developBranch);

        var featurePrefix = _sysConsole
            .Write($"Prefix for feature branches (Empty = current value '{configuration.FeatureBranchPrefix}')")
            .ReadLine();

        if (!string.IsNullOrEmpty(featurePrefix))
        {
            configuration.FeatureBranchPrefix = featurePrefix;
        }

        var gitProviderName = _sysConsole
            .WriteLine("Select git service provider")
            .SelectItem(_gitServiceProviders.ProviderNames);

        if (!string.IsNullOrEmpty(gitProviderName))
        {
            configuration.GitServiceProviderName = gitProviderName;
        }

        await _repositoryConfigurations.SaveConfigurationAsync(repository.Info.RemoteUri, configuration);
    }

    // ReSharper disable once StringLiteralTypo
    [CliAction("showconfig")]
    public void ShowConfig()
    {
        using var repository = _gitRepositoryFactory.OpenRepository(Env.CurrentDirectory);

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        _sysConsole
            .WriteLine()
            .WriteLine($"Configuration for '{repository.Info.RemoteUri}'")
            .WriteLine()
            .WriteLine($"HasDevelopBranch: {configuration.HasDevelopBranch}")
            .WriteLine($"DevelopBranch: {configuration.DevelopBranch}")
            .WriteLine($"FeatureBranchPrefix: {configuration.FeatureBranchPrefix}")
            .WriteLine($"GitServiceProviderName: {configuration.GitServiceProviderName}")
            .WriteLine();
    }
}
