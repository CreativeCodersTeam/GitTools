namespace CreativeCoders.GitTool.Base.Output;

public interface ICml
{
    string Text(string text);

    string LightText(string text);

    string Caption(string text);

    string Warning(string text);

    string HighLight(string text);

    string Url(string text);
}
