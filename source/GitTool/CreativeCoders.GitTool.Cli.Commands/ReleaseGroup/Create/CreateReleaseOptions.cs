using CreativeCoders.Cli.Core;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.Create;

[PublicAPI]
public class CreateReleaseOptions : IOptionsValidation
{
    private const string PushAllTagsLongName = "alltags";

    [OptionValue(0, IsRequired = false, HelpText = "Version for release. If set version increment is not allowed.")]
    public string? Version { get; set; }

    [OptionParameter('a', PushAllTagsLongName)]
    public bool PushAllTags { get; set; }

    [OptionParameter('i', "increment", HelpText = "Version auto increment. If set version is not allowed.")]
    public VersionAutoIncrement? VersionIncrement { get; set; }

    [OptionParameter('r', "resetlower", HelpText = "Reset lower version parts on auto increment")]
    public bool ResetLowerVersionPartsOnAutoInc { get; set; } = true;

    [OptionParameter("nc", "noconfirm", HelpText = "No confirmation for auto increment version")]
    public bool NoConfirmAutoIncrementVersion { get; set; }

    public Task<OptionsValidationResult> ValidateAsync()
    {
        if (string.IsNullOrWhiteSpace(Version) && VersionIncrement == null)
        {
            return Task.FromResult(OptionsValidationResult.Invalid(["Version or version increment must be specified"]));
        }

        if (!string.IsNullOrWhiteSpace(Version) && VersionIncrement != null)
        {
            return Task.FromResult(
                OptionsValidationResult.Invalid(["Version and version increment are mutually exclusive"]));
        }

        return Task.FromResult(OptionsValidationResult.Valid());
    }
}
