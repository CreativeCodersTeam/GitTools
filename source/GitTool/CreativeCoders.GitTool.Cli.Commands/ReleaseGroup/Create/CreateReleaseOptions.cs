using CreativeCoders.Cli.Core;
using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.ReleaseGroup.Create;

[PublicAPI]
public class CreateReleaseOptions : IOptionsValidation
{
    private const string PushAllTagsLongName = "alltags";

    [OptionValue(0, IsRequired = false)] public string? Version { get; set; }

    [OptionParameter('a', PushAllTagsLongName)]
    public bool PushAllTags { get; set; }

    [OptionParameter('i', "increment", HelpText = "Version increment")]
    public VersionAutoIncrement? VersionIncrement { get; set; }

    [OptionParameter('r', "resetlower", HelpText = "Reset lower version parts on auto increment")]
    public bool ResetLowerVersionPartsOnAutoInc { get; set; } = true;

    [OptionParameter('c', "confirm", HelpText = "Confirm auto increment version")]
    public bool ConfirmAutoIncrementVersion { get; set; }

    public Task<OptionsValidationResult> ValidateAsync()
    {
        if (string.IsNullOrWhiteSpace(Version) && VersionIncrement == null)
        {
            return Task.FromResult(OptionsValidationResult.Invalid(["Version or version increment must be specified"]));
        }

        return Task.FromResult(OptionsValidationResult.Valid());
    }
}
