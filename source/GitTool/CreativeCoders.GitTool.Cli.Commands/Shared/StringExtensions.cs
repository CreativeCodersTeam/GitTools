namespace CreativeCoders.GitTool.Cli.Commands.Shared;

public static class StringExtensions
{
    public static string ToErrorMarkup(this string text)
    {
        return $"[red]{text}[/]";
    }

    public static string ToSuccessMarkup(this string text)
    {
        return $"[green]{text}[/]";
    }

    public static string ToWarningMarkup(this string text)
    {
        return $"[yellow]{text}[/]";
    }
}
