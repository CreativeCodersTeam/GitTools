using CreativeCoders.Cli.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Configurations;
using CreativeCoders.SysConsole.Core;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli.Commands.Tool.Setup;

[UsedImplicitly]
[CliCommand(["setup"], Description = "Setup the git tool for the current repository")]
public class SetupCommand(
    IAnsiConsole ansiConsole,
    IGitRepository gitRepository,
    IRepositoryConfigurations repositoryConfigurations,
    IGitServiceProviders gitServiceProviders) : ICliCommand
{
    public async Task<CommandResult> ExecuteAsync()
    {
        var configuration = repositoryConfigurations.GetConfiguration(gitRepository);

        ansiConsole.PrintBlock()
            .WriteLine()
            .WriteLine("Development branch (empty = no development branch)");

        var developBranch =
            await ansiConsole.PromptAsync(
                new TextPrompt<string?>($"(Current value = '{configuration.DevelopBranch}'): ")
                    { AllowEmpty = true });

        configuration.DevelopBranch = developBranch ?? string.Empty;
        configuration.HasDevelopBranch = !string.IsNullOrEmpty(developBranch);

        var featureBranchPrefix = await ansiConsole.PromptAsync(new TextPrompt<string?>(
                $"Prefix for feature branches (Empty = current value '{configuration.FeatureBranchPrefix}'): ")
            { AllowEmpty = true });

        if (!string.IsNullOrEmpty(featureBranchPrefix))
        {
            configuration.FeatureBranchPrefix = featureBranchPrefix;
        }

        var selectionPrompt = new SelectionPrompt<string>();

        var gitProviderName =
            await ansiConsole.PromptAsync(selectionPrompt.AddChoices(gitServiceProviders.ProviderNames));

        if (!string.IsNullOrEmpty(gitProviderName))
        {
            configuration.GitServiceProviderName = gitProviderName;
        }

        var disableCertValidationPrompt = new ConfirmationPrompt("Disable certificate check (true/false): ")
            { DefaultValue = configuration.DisableCertificateValidation };

        configuration.DisableCertificateValidation = await ansiConsole.PromptAsync(disableCertValidationPrompt);

        await repositoryConfigurations.SaveConfigurationAsync(gitRepository.Info.RemoteUri, configuration);

        return CommandResult.Success;
    }
}
