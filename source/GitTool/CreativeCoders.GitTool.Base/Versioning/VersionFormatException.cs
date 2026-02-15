using System;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Base.Versioning;

[PublicAPI]
public class VersionFormatException(string? version, string? message = null) : Exception(message ??
    $"The version '{version}' has an invalid format. The version must consist of 1 to 3 numeric parts separated by dots (e.g. '1.0.0').")
{
    public string? Version { get; } = version;
}
