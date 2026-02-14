using System.Diagnostics.CodeAnalysis;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Tags;

namespace CreativeCoders.GitTool.Base.Versioning;

[ExcludeFromCodeCoverage]
public class VersionTag(string version, IGitTag tag)
{
    public string Version { get; } = Ensure.NotNull(version);

    public IGitTag Tag { get; } = Ensure.NotNull(tag);
}
