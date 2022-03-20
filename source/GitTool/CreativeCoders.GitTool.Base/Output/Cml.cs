namespace CreativeCoders.GitTool.Base.Output;

public class Cml : ICml
{
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
        return text;
    }

    public string HighLight(string text)
    {
        return $"[bold lime]{text}[/]";
    }

    public string Url(string text)
    {
        return text;
    }
}