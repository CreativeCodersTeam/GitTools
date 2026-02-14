using System.Collections.Generic;
using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Base.Versioning;

public static class GitRepositoryExtensions
{
    public static IEnumerable<VersionTag> GetVersionTags(this IGitRepository gitRepository)
    {
        foreach (var tag in gitRepository.Tags)
        {
            if (VersionUtils.IsValidVersion(tag.Name.Friendly, out var version))
            {
                yield return new VersionTag(version, tag);
            }
        }
    }
}
