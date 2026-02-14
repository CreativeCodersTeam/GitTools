using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Base;

public class VersionBuilder
{
    private const int MajorPartIndex = 0;
    private const int MinorPartIndex = 1;
    private const int PatchPartIndex = 2;

    private readonly VersionFormatKind _formatKind;

    private readonly List<string> _versionParts = [];

    public VersionBuilder(string version, VersionFormatKind formatKind = VersionFormatKind.Strict)
    {
        _formatKind = formatKind;
        _versionParts.AddRange(SplitVersionParts(version));
    }

    private IEnumerable<string> SplitVersionParts(string version)
    {
        var versionParts = version.Split('.').ToList();

        if (versionParts.Count > 3)
        {
            throw new VersionFormatException(version);
        }

        for (var i = 0; i < versionParts.Count; i++)
        {
            var versionPart = versionParts[i];

            if (versionPart.Trim() != versionPart && _formatKind == VersionFormatKind.Strict)
            {
                throw new VersionFormatException(version,
                    $"The version part '{versionPart}' contains leading or trailing whitespace.");
            }

            if (!int.TryParse(versionPart.Trim(), out var versionPartInt))
            {
                throw new VersionFormatException(version, $"The version part '{versionPart}' is not a valid integer.");
            }

            versionParts[i] = versionPartInt.ToString();
        }

        while (versionParts.Count < 3)
        {
            versionParts.Add("0");
        }

        return versionParts;
    }

    private static string BuildVersion(IEnumerable<string> versionParts)
    {
        return string.Join(".", versionParts);
    }

    public void IncrementPatch(int incrementBy = 1)
    {
        IncrementPart(PatchPartIndex, incrementBy);
    }

    public void IncrementMinor(int incrementBy = 1)
    {
        IncrementPart(MinorPartIndex, incrementBy);
    }

    public void IncrementMajor(int incrementBy = 1)
    {
        IncrementPart(MajorPartIndex, incrementBy);
    }

    private void IncrementPart(int partIndex, int incrementBy)
    {
        if (!int.TryParse(_versionParts[partIndex], out var versionPartNumber))
        {
            throw new InvalidOperationException();
        }

        _versionParts[partIndex] = (versionPartNumber + incrementBy).ToString();
    }

    public string Build()
    {
        return BuildVersion(_versionParts);
    }
}

public enum VersionFormatKind
{
    Strict,
    Loose
}

[PublicAPI]
public class VersionFormatException(string? version, string? message = null) : Exception(message ??
    $"The version '{version}' has an invalid format. The version must consist of at least 3 parts separated by dots (e.g. '1.0.0').")
{
    public string? Version { get; } = version;
}
