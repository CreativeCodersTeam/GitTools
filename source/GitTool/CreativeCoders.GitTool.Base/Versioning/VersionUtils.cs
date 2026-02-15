using System;
using System.Linq;

namespace CreativeCoders.GitTool.Base.Versioning;

public static class VersionUtils
{
    public static bool IsValidVersion(string version, out string normalizedVersion,
        bool ignoreLeadingVersionPrefix = true)
    {
        if (ignoreLeadingVersionPrefix)
        {
            version = RemoveLeadingVersionPrefix(version);
        }

        var versionParts = version.Split('.');

        var isValidVersion = versionParts.All(x => int.TryParse(x, out _));

        normalizedVersion = isValidVersion ? string.Join(".", versionParts) : string.Empty;

        return isValidVersion;
    }

    public static string RemoveLeadingVersionPrefix(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return string.Empty;
        }

        string[] versionPrefixes = ["version", "v"];

        var versionPrefix =
            versionPrefixes.FirstOrDefault(x => version.StartsWith(x, StringComparison.OrdinalIgnoreCase));

        return string.IsNullOrWhiteSpace(versionPrefix)
            ? version
            : version[versionPrefix.Length..];
    }
}
