using Spectre.Console;

namespace CreativeCoders.GitTool.Base.Output;

public static class AnsiConsoleExtensions
{
    public static IAnsiConsole EmptyLine(this IAnsiConsole ansiConsole)
    {
        ansiConsole.WriteLine();

        return ansiConsole;
    }

    public static IAnsiConsole WriteLineEx(this IAnsiConsole ansiConsole, string text)
    {
        ansiConsole.WriteLine(text);

        return ansiConsole;
    }

    public static IAnsiConsole WriteMarkupLine(this IAnsiConsole ansiConsole, string markup)
    {
        ansiConsole.Write(new Markup(markup));

        return ansiConsole.EmptyLine();
    }
}
