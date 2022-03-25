using Spectre.Console;

namespace CreativeCoders.GitTool.Base.Output;

public class Cml : ICml
{
    private readonly IAnsiConsole _ansiConsole;

    public Cml(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
    }

    public string Text(string text)
    {
        return text;
    }

    public string LightText(string text)
    {
        return $"[bold teal]{text}[/]";
    }

    public string Caption(string text)
    {
        return $"[bold white]{text}[/]";
    }

    public string Warning(string text)
    {
        return $"[bold italic yellow]{text}[/]";
    }

    public string Error(string text)
    {
        return $"[bold italic red3]{text}[/]";
    }

    public string HighLight(string text)
    {
        return $"[bold lime]{text}[/]";
    }

    public string Url(string url)
    {
        return _ansiConsole.Profile.Capabilities.Links
            ? $"[link={url}]{url}[/]"
            : $"[underline aqua]{url}[/]";
    }
}