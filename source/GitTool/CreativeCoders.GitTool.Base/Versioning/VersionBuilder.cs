using System;
using System.Collections.Generic;
using System.Linq;

namespace CreativeCoders.GitTool.Base.Versioning;

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

    private List<string> SplitVersionParts(string version)
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

    public VersionBuilder IncrementPatch(int incrementBy = 1)
    {
        return IncrementPart(PatchPartIndex, incrementBy);
    }

    public VersionBuilder IncrementMinor(int incrementBy = 1)
    {
        return IncrementPart(MinorPartIndex, incrementBy);
    }

    public VersionBuilder IncrementMajor(int incrementBy = 1)
    {
        return IncrementPart(MajorPartIndex, incrementBy);
    }

    private VersionBuilder IncrementPart(int partIndex, int incrementBy)
    {
        if (!int.TryParse(_versionParts[partIndex], out var versionPartNumber))
        {
            throw new InvalidOperationException();
        }

        _versionParts[partIndex] = (versionPartNumber + incrementBy).ToString();

        return this;
    }

    public string Build()
    {
        return BuildVersion(_versionParts);
    }

    public int GetVersionPart(int partIndex)
    {
        if (partIndex < 0 || partIndex > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(partIndex));
        }

        return int.Parse(_versionParts[partIndex]);
    }
}
