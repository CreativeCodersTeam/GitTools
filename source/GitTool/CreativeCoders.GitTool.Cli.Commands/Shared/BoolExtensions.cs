namespace CreativeCoders.GitTool.Cli.Commands.Shared;

public static class BoolExtensions
{
    public static void IfElse(this bool condition, Action trueAction, Action falseAction)
    {
        if (condition)
        {
            trueAction();
        }
        else
        {
            falseAction();
        }
    }

    public static T IfElse<T>(this bool condition, Func<T> trueFunc, Func<T> falseFunc)
    {
        return condition ? trueFunc() : falseFunc();
    }

    public static bool If(this bool condition, Action action)
    {
        if (!condition)
        {
            return false;
        }

        action();
        return true;
    }

    public static void Else(this bool condition, Action action)
    {
        if (condition)
        {
            return;
        }

        action();
    }
}
