using System.Collections.Generic;

namespace CreativeCoders.GitTool.Base.PullRequestInfos;

internal sealed record CommitTypeMetadata(
    string Type,
    string Header,
    string Description,
    int Priority)
{
    private static readonly Dictionary<string, CommitTypeMetadata> KnownTypes = new()
    {
        ["feat"] = new CommitTypeMetadata("feat", "✨ Features", "new features", 0),
        ["fix"] = new CommitTypeMetadata("fix", "🐛 Bug Fixes", "bug fixes", 1),
        ["perf"] = new CommitTypeMetadata("perf", "⚡ Performance", "performance improvements", 2),
        ["refactor"] = new CommitTypeMetadata("refactor", "♻️ Refactoring", "refactoring", 3),
        ["docs"] = new CommitTypeMetadata("docs", "📚 Documentation", "documentation updates", 4),
        ["test"] = new CommitTypeMetadata("test", "✅ Tests", "test improvements", 5),
        ["build"] = new CommitTypeMetadata("build", "🔧 Build", "build changes", 6),
        ["ci"] = new CommitTypeMetadata("ci", "👷 CI/CD", "CI/CD changes", 7),
        ["chore"] = new CommitTypeMetadata("chore", "🔨 Chores", "maintenance tasks", 8),
        ["style"] = new CommitTypeMetadata("style", "💎 Style", "code style changes", 9),
        ["revert"] = new CommitTypeMetadata("revert", "⏪ Reverts", "reverts", 10)
    };

    private const int UnknownTypePriority = 99;

    public static CommitTypeMetadata GetMetadata(string type)
    {
        if (KnownTypes.TryGetValue(type, out var metadata))
        {
            return metadata;
        }

        return CreateUnknownTypeMetadata(type);
    }

    public static IEnumerable<string> GetMaintenanceTypes()
    {
        return new[] { "chore", "build", "ci" };
    }

    private static CommitTypeMetadata CreateUnknownTypeMetadata(string type)
    {
        var capitalizedType = CapitalizeFirstLetter(type);
        return new CommitTypeMetadata(
            type,
            $"📝 {capitalizedType}",
            type,
            UnknownTypePriority);
    }

    private static string CapitalizeFirstLetter(string text)
    {
        return text.Length > 0
            ? $"{char.ToUpper(text[0])}{text[1..]}"
            : text;
    }
}
