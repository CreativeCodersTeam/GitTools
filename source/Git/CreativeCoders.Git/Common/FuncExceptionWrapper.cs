namespace CreativeCoders.Git.Common;

[PublicAPI]
public class FuncExceptionWrapper<TException, TResult>
    where TException : Exception
{
    private readonly Func<TResult> _func;

    private Func<TException, Exception>? _createWrapperException;

    public FuncExceptionWrapper(Func<TResult> func)
    {
        _func = func;
    }

    public FuncExceptionWrapper<TException, TResult> Wrap(Func<TException, Exception> createWrapperException)
    {
        _createWrapperException = createWrapperException;

        return this;
    }

    public FuncExceptionWrapper<TException2, TResult> AndWrap<TException2>(
        Func<TException2, Exception> createWrapperException)
        where TException2 : Exception
    {
        return new FuncExceptionWrapper<TException2, TResult>(Execute).Wrap(createWrapperException);
    }

    public TResult Execute()
    {
        try
        {
            return _func();
        }
        catch (TException e)
        {
            throw _createWrapperException?.Invoke(e) ?? e;
        }
    }
}