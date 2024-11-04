using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Tool;

[PublicAPI]
[CliController]
public class ToolController
{
    private readonly IAnsiConsole _console;

    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly IGitServiceProviders _gitServiceProviders;

    private readonly IRepositoryConfigurations _repositoryConfigurations;

    public ToolController(IAnsiConsole console, IRepositoryConfigurations repositoryConfigurations,
        IGitRepositoryFactory gitRepositoryFactory, IGitServiceProviders gitServiceProviders)
    {
        _console = Ensure.NotNull(console);
        _gitServiceProviders = Ensure.NotNull(gitServiceProviders);
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory);
        _repositoryConfigurations = Ensure.NotNull(repositoryConfigurations);
    }

    [CliAction("setup")]
    public async Task Setup()
    {
        using var repository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        _console.PrintBlock()
            .WriteLine()
            .WriteLine("Development branch (empty = no development branch)");

        var developBranch =
            _console.Prompt(new TextPrompt<string?>($"(Current value = '{configuration.DevelopBranch}'): ")
                { AllowEmpty = true });

        configuration.DevelopBranch = developBranch ?? string.Empty;
        configuration.HasDevelopBranch = !string.IsNullOrEmpty(developBranch);

        var featureBranchPrefix = _console.Prompt(new TextPrompt<string?>(
                $"Prefix for feature branches (Empty = current value '{configuration.FeatureBranchPrefix}'): ")
            { AllowEmpty = true });

        if (!string.IsNullOrEmpty(featureBranchPrefix))
        {
            configuration.FeatureBranchPrefix = featureBranchPrefix;
        }

        var selectionPrompt = new SelectionPrompt<string>();

        var gitProviderName = _console.Prompt(selectionPrompt.AddChoices(_gitServiceProviders.ProviderNames));

        if (!string.IsNullOrEmpty(gitProviderName))
        {
            configuration.GitServiceProviderName = gitProviderName;
        }

        var disableCertValidationPrompt = new ConfirmationPrompt("Disable certificate check (true/false): ")
            { DefaultValue = configuration.DisableCertificateValidation };

        configuration.DisableCertificateValidation = _console.Prompt(disableCertValidationPrompt);

        await _repositoryConfigurations.SaveConfigurationAsync(repository.Info.RemoteUri, configuration);
    }

    // ReSharper disable once StringLiteralTypo
    [CliAction("showconfig")]
    public void ShowConfig()
    {
        using var repository = _gitRepositoryFactory.OpenRepository(Env.CurrentDirectory);

        var configuration = _repositoryConfigurations.GetConfiguration(repository);

        _console.PrintBlock()
            .WriteLine()
            .WriteLine($"Configuration for '{repository.Info.RemoteUri}'")
            .WriteLine()
            .WriteLine($"HasDevelopBranch: {configuration.HasDevelopBranch}")
            .WriteLine($"DevelopBranch: {configuration.DevelopBranch}")
            .WriteLine($"FeatureBranchPrefix: {configuration.FeatureBranchPrefix}")
            .WriteLine($"GitServiceProviderName: {configuration.GitServiceProviderName}")
            .WriteLine($"DisableCertificateValidation: {configuration.DisableCertificateValidation}")
            .WriteLine();
    }
}
