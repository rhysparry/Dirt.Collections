namespace Dirt.Collections;

/// <summary>
/// A stack-based collection for managing the asynchronous disposal of <see cref="IAsyncDisposable"/> objects.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="AsyncDisposableStack"/> provides a convenient way to ensure that multiple asynchronously disposable objects
/// are disposed in reverse order of their addition (LIFO - Last In, First Out). This is particularly useful
/// when managing async resources that depend on each other and must be cleaned up in a specific order.
/// </para>
/// <para>
/// The stack supports both <see cref="IAsyncDisposable"/> and <see cref="IDisposable"/> objects through overloaded
/// <see cref="Push(IDisposable)"/> and <see cref="Push(IAsyncDisposable)"/> methods. Synchronous disposable objects are automatically wrapped to work with the async disposal pattern.
/// </para>
/// <para>
/// When <see cref="IAsyncDisposable.DisposeAsync"/> is called, all objects in the stack are disposed asynchronously in reverse order of insertion.
/// If any dispose operation throws an exception, all remaining objects are still disposed, and the exceptions
/// are collected and thrown as an <see cref="AggregateException"/> after all disposal attempts.
/// </para>
/// </remarks>
public sealed class AsyncDisposableStack : IAsyncDisposable
{
    private readonly Stack<IAsyncDisposable> _toDispose;
    private bool _disposing;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDisposableStack"/> class.
    /// </summary>
    public AsyncDisposableStack()
    {
        _toDispose = new Stack<IAsyncDisposable>();
    }

    /// <summary>
    /// Adds an asynchronously disposable object to the stack.
    /// </summary>
    /// <param name="disposable">The asynchronously disposable object to add to the stack.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="disposable"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the stack has already been disposed.</exception>
    public void Push(IAsyncDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);
        if (_disposing)
        {
            throw new InvalidOperationException(
                "Cannot re-use an AsyncDisposableStack after DisposeAsync."
            );
        }
        _toDispose.Push(disposable);
    }

    /// <summary>
    /// Adds a synchronously disposable object to the stack.
    /// </summary>
    /// <remarks>
    /// If the object also implements <see cref="IAsyncDisposable"/>, the async version is used directly.
    /// Otherwise, the synchronous disposable is automatically wrapped to work with the async disposal pattern.
    /// </remarks>
    /// <param name="disposable">The synchronously disposable object to add to the stack.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="disposable"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the stack has already been disposed.</exception>
    public void Push(IDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);
        if (disposable is IAsyncDisposable asyncDisposable)
        {
            Push(asyncDisposable);
            return;
        }
        var wrapper = new DisposableWrapper(disposable);
        Push(wrapper);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
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
                await item.DisposeAsync();
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

    sealed class DisposableWrapper(IDisposable toDispose) : IAsyncDisposable
    {
        private bool _disposing;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_disposing)
            {
                return ValueTask.CompletedTask;
            }

            _disposing = true;

            toDispose.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
