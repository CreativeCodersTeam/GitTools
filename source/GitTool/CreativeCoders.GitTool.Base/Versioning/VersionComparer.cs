using System.Collections.Generic;

namespace CreativeCoders.GitTool.Base.Versioning;

public class VersionComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        switch (x)
        {
            case null when y == null:
                return 0;
            case null:
                return -1;
        }

        if (y == null)
        {
            return 1;
        }

        var versionBuilderX = new VersionBuilder(x);
        var versionBuilderY = new VersionBuilder(y);

        for (var i = 0; i < 3; i++)
        {
            var versionPartX = versionBuilderX.GetVersionPart(i);
            var versionPartY = versionBuilderY.GetVersionPart(i);

            var versionPartComparison = versionPartX.CompareTo(versionPartY);

            if (versionPartComparison != 0)
            {
                return versionPartComparison;
            }
        }

        return 0;
    }
}
