using JetBrains.Annotations;

namespace CreativeCoders.Git.Common;

[PublicAPI]
public class ActionExceptionWrapper<TException>
    where TException : Exception
{
    private readonly Action _action;

    private Func<TException, Exception>? _createWrapperException;

    public ActionExceptionWrapper(Action action)
    {
        _action = action;
    }

    public ActionExceptionWrapper<TException> Wrap(Func<TException, Exception> createWrapperException)
    {
        _createWrapperException = createWrapperException;

        return this;
    }

    public ActionExceptionWrapper<TException2> AndWrap<TException2>(
        Func<TException2, Exception> createWrapperException)
        where TException2 : Exception
    {
        return new ActionExceptionWrapper<TException2>(Execute).Wrap(createWrapperException);
    }

    public void Execute()
    {
        try
        {
            _action();
        }
        catch (TException e)
        {
            throw _createWrapperException?.Invoke(e) ?? e;
        }
    }
}