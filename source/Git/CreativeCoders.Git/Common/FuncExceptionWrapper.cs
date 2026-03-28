namespace CreativeCoders.Git.Common;

/// <summary>
/// Wraps a <see cref="Func{TResult}"/> and translates a specific exception type into a different exception.
/// </summary>
/// <typeparam name="TException">The type of exception to catch and wrap.</typeparam>
/// <typeparam name="TResult">The return type of the wrapped function.</typeparam>
[PublicAPI]
public class FuncExceptionWrapper<TException, TResult>
    where TException : Exception
{
    private readonly Func<TResult> _func;

    private Func<TException, Exception>? _createWrapperException;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncExceptionWrapper{TException, TResult}"/> class.
    /// </summary>
    /// <param name="func">The function to wrap.</param>
    public FuncExceptionWrapper(Func<TResult> func)
    {
        _func = func;
    }

    /// <summary>
    /// Configures the factory function that creates the wrapper exception from the caught exception.
    /// </summary>
    /// <param name="createWrapperException">A function that creates the wrapper exception.</param>
    /// <returns>The current instance for fluent chaining.</returns>
    public FuncExceptionWrapper<TException, TResult> Wrap(Func<TException, Exception> createWrapperException)
    {
        _createWrapperException = createWrapperException;

        return this;
    }

    /// <summary>
    /// Chains an additional exception wrapper for a different exception type.
    /// </summary>
    /// <typeparam name="TException2">The type of exception to catch in the chained wrapper.</typeparam>
    /// <param name="createWrapperException">A function that creates the wrapper exception.</param>
    /// <returns>A new <see cref="FuncExceptionWrapper{TException2, TResult}"/> wrapping this instance.</returns>
    public FuncExceptionWrapper<TException2, TResult> AndWrap<TException2>(
        Func<TException2, Exception> createWrapperException)
        where TException2 : Exception
    {
        return new FuncExceptionWrapper<TException2, TResult>(Execute).Wrap(createWrapperException);
    }

    /// <summary>
    /// Executes the wrapped function, catching and translating the configured exception type.
    /// </summary>
    /// <returns>The result of the wrapped function.</returns>
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