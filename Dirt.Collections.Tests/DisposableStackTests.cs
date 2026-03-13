namespace Dirt.Collections.Tests;

public class DisposableStackTests
{
    [Fact]
    public void Push_SingleDisposable_DisposesIt()
    {
        var disposable = new MockDisposable();

        using (var stack = new DisposableStack())
        {
            stack.Push(disposable);
        }

        Assert.True(disposable.Disposed);
    }

    [Fact]
    public void Push_MultipleDisposables_DisposesInReverseOrder()
    {
        var disposables = new[]
        {
            new MockDisposable(),
            new MockDisposable(),
            new MockDisposable(),
        };
        var disposeOrder = new List<MockDisposable>();

        foreach (var d in disposables)
        {
            d.DisposeAction = () => disposeOrder.Add(d);
        }

        using (var stack = new DisposableStack())
        {
            foreach (var d in disposables)
            {
                stack.Push(d);
            }
        }

        Assert.Equal(new[] { disposables[2], disposables[1], disposables[0] }, disposeOrder);
    }

    [Fact]
    public void Dispose_MultipleDisposables_WithException_DisposesAllAndThrowsAggregate()
    {
        var disposables = new[]
        {
            new MockDisposable { ShouldThrow = true },
            new MockDisposable(),
            new MockDisposable { ShouldThrow = true },
        };

        using var stack = new DisposableStack();
        foreach (var d in disposables)
        {
            stack.Push(d);
        }

        var ex = Assert.Throws<AggregateException>(() => ((IDisposable)stack).Dispose());

        Assert.Equal(2, ex.InnerExceptions.Count);
        foreach (var d in disposables)
        {
            Assert.True(d.Disposed);
        }
    }

    [Fact]
    public void Dispose_CalledTwice_SecondCallDoesNothing()
    {
        var disposable = new MockDisposable();

        var stack = new DisposableStack();
        var d = (IDisposable)stack;
        stack.Push(disposable);

        // First dispose will dispose the pushed item
        d.Dispose();

        // Second dispose should be a no-op with respect to the item
        d.Dispose();

        Assert.True(disposable.Disposed);
        Assert.Equal(1, disposable.DisposeCount);
    }

    [Fact]
    public void Push_AfterDispose_Throws()
    {
        var disposable = new MockDisposable();
        var stack = new DisposableStack();
        var d = (IDisposable)stack;
        d.Dispose();
        Assert.Throws<InvalidOperationException>(() => stack.Push(disposable));
    }

    [Fact]
    public void Dispose_EmptyStack_DoesNotThrow()
    {
        using var stack = new DisposableStack();
    }

    [Fact]
    public void Push_WithNull_ThrowsArgumentNullException()
    {
        var stack = new DisposableStack();

        Assert.Throws<ArgumentNullException>(() => stack.Push(null!));
    }

    private sealed class MockDisposable : IDisposable
    {
        public bool Disposed { get; private set; }
        public int DisposeCount { get; private set; }
        public bool ShouldThrow { get; init; }
        public Action? DisposeAction { get; set; }

        public void Dispose()
        {
            Disposed = true;
            DisposeCount++;
            DisposeAction?.Invoke();

            if (ShouldThrow)
            {
                throw new TestException();
            }
        }
    }

    private sealed class TestException : Exception { }
}
