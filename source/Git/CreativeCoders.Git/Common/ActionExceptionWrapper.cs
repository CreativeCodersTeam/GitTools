namespace CreativeCoders.Git.Common;

/// <summary>
/// Wraps an <see cref="Action"/> and translates a specific exception type into a different exception.
/// </summary>
/// <typeparam name="TException">The type of exception to catch and wrap.</typeparam>
[PublicAPI]
public class ActionExceptionWrapper<TException>
    where TException : Exception
{
    private readonly Action _action;

    private Func<TException, Exception>? _createWrapperException;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExceptionWrapper{TException}"/> class.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    public ActionExceptionWrapper(Action action)
    {
        _action = action;
    }

    /// <summary>
    /// Configures the factory function that creates the wrapper exception from the caught exception.
    /// </summary>
    /// <param name="createWrapperException">A function that creates the wrapper exception.</param>
    /// <returns>The current instance for fluent chaining.</returns>
    public ActionExceptionWrapper<TException> Wrap(Func<TException, Exception> createWrapperException)
    {
        _createWrapperException = createWrapperException;

        return this;
    }

    /// <summary>
    /// Chains an additional exception wrapper for a different exception type.
    /// </summary>
    /// <typeparam name="TException2">The type of exception to catch in the chained wrapper.</typeparam>
    /// <param name="createWrapperException">A function that creates the wrapper exception.</param>
    /// <returns>A new <see cref="ActionExceptionWrapper{TException2}"/> wrapping this instance.</returns>
    public ActionExceptionWrapper<TException2> AndWrap<TException2>(
        Func<TException2, Exception> createWrapperException)
        where TException2 : Exception
    {
        return new ActionExceptionWrapper<TException2>(Execute).Wrap(createWrapperException);
    }

    /// <summary>
    /// Executes the wrapped action, catching and translating the configured exception type.
    /// </summary>
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