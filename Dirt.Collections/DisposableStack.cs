namespace Dirt.Collections;

/// <summary>
/// A stack-based collection for managing the disposal of <see cref="IDisposable"/> objects.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="DisposableStack"/> provides a convenient way to ensure that multiple disposable objects
/// are disposed in reverse order of their addition (LIFO - Last In, First Out). This is particularly useful
/// when managing resources that depend on each other and must be cleaned up in a specific order.
/// </para>
/// <para>
/// When <see cref="IDisposable.Dispose"/> is called, all objects in the stack are disposed in reverse order of insertion.
/// If any dispose operation throws an exception, all remaining objects are still disposed, and the exceptions
/// are collected and thrown as an <see cref="AggregateException"/> after all disposal attempts.
/// </para>
/// </remarks>
public sealed class DisposableStack : IDisposable
{
    private readonly Stack<IDisposable> _toDispose;
    private bool _disposing;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableStack"/> class.
    /// </summary>
    public DisposableStack()
    {
        _toDispose = new Stack<IDisposable>();
    }

    /// <summary>
    /// Adds a disposable object to the stack.
    /// </summary>
    /// <param name="disposable">The disposable object to add to the stack.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="disposable"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the stack has already been disposed.</exception>
    public void Push(IDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);
        if (_disposing)
        {
            throw new InvalidOperationException("Cannot re-use a DisposableStack after Dispose.");
        }

        _toDispose.Push(disposable);
    }

    void IDisposable.Dispose()
    {
        if (_disposing)
        {
            return;
        }

        _disposing = true;

        var exceptions = new List<Exception>();

        while (_toDispose.TryPop(out var item))
        {
            try
            {
                item.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}
