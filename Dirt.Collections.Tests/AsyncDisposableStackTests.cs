namespace Dirt.Collections.Tests;

public class AsyncDisposableStackTests
{
    [Fact]
    public async Task Push_SingleAsyncDisposable_DisposesIt()
    {
        var disposable = new MockAsyncDisposable();

        await using (var stack = new AsyncDisposableStack())
        {
            stack.Push(disposable);
        }

        Assert.True(disposable.Disposed);
    }

    [Fact]
    public async Task Push_MultipleAsyncDisposables_DisposesInReverseOrder()
    {
        var disposables = new[]
        {
            new MockAsyncDisposable(),
            new MockAsyncDisposable(),
            new MockAsyncDisposable(),
        };
        var disposeOrder = new List<MockAsyncDisposable>();

        foreach (var d in disposables)
        {
            d.DisposeAction = () => disposeOrder.Add(d);
        }

        await using (var stack = new AsyncDisposableStack())
        {
            foreach (var d in disposables)
            {
                stack.Push(d);
            }
        }

        Assert.Equal(new[] { disposables[2], disposables[1], disposables[0] }, disposeOrder);
    }

    [Fact]
    public async Task Push_SyncDisposable_WrapsAndDisposesIt()
    {
        var disposable = new MockDisposable();

        await using (var stack = new AsyncDisposableStack())
        {
            stack.Push(disposable);
        }

        Assert.True(disposable.Disposed);
    }

    [Fact]
    public async Task Push_SyncDisposable_WithAsyncInterface_UsesAsyncDirectly()
    {
        var disposable = new MockDualDisposable();

        await using (var stack = new AsyncDisposableStack())
        {
            stack.Push((IDisposable)disposable);
        }

        Assert.True(disposable.AsyncDisposed);
        Assert.False(disposable.SyncDisposed);
    }

    [Fact]
    public async Task Push_MultipleDisposables_MixedTypes_DisposesInReverseOrder()
    {
        var asyncDisposable = new MockAsyncDisposable();
        var syncDisposable = new MockDisposable();
        var dualDisposable = new MockDualDisposable();

        await using (var stack = new AsyncDisposableStack())
        {
            stack.Push(asyncDisposable);
            stack.Push(syncDisposable);
            stack.Push((IDisposable)dualDisposable);
        }

        Assert.True(asyncDisposable.Disposed);
        Assert.True(syncDisposable.Disposed);
        Assert.True(dualDisposable.AsyncDisposed);
    }

    [Fact]
    public async Task DisposeAsync_MultipleDisposables_WithException_DisposesAllAndThrowsAggregate()
    {
        var disposables = new[]
        {
            new MockAsyncDisposable { ShouldThrow = true },
            new MockAsyncDisposable(),
            new MockAsyncDisposable { ShouldThrow = true },
        };

        await using var stack = new AsyncDisposableStack();
        foreach (var d in disposables)
        {
            stack.Push(d);
        }

        var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
            await ((IAsyncDisposable)stack).DisposeAsync()
        );

        Assert.Equal(2, ex.InnerExceptions.Count);
        foreach (var d in disposables)
        {
            Assert.True(d.Disposed);
        }
    }

    [Fact]
    public async Task DisposeAsync_CalledTwice_SecondCallDoesNothing()
    {
        var disposable = new MockAsyncDisposable();

        var stack = new AsyncDisposableStack();
        IAsyncDisposable d = stack;
        stack.Push(disposable);

        await d.DisposeAsync();
        await d.DisposeAsync();

        Assert.Equal(1, disposable.DisposeCount);
    }

    [Fact]
    public async Task DisposeAsync_EmptyStack_DoesNotThrow()
    {
        await using var stack = new AsyncDisposableStack();
    }

    [Fact]
    public void Push_AsyncDisposable_WithNull_ThrowsArgumentNullException()
    {
        var stack = new AsyncDisposableStack();

        Assert.Throws<ArgumentNullException>(() => stack.Push((IAsyncDisposable)null!));
    }

    [Fact]
    public void Push_Disposable_WithNull_ThrowsArgumentNullException()
    {
        var stack = new AsyncDisposableStack();

        Assert.Throws<ArgumentNullException>(() => stack.Push((IDisposable)null!));
    }

    [Fact]
    public async Task Push_AsyncDisposable_AfterDisposeAsync_Throws()
    {
        var disposable = new MockAsyncDisposable();
        var stack = new AsyncDisposableStack();
        IAsyncDisposable d = stack;
        await d.DisposeAsync();
        Assert.Throws<InvalidOperationException>(() => stack.Push(disposable));
    }

    [Fact]
    public async Task Push_Disposable_AfterDisposeAsync_Throws()
    {
        var disposable = new MockDisposable();
        var stack = new AsyncDisposableStack();
        IAsyncDisposable d = stack;
        await d.DisposeAsync();
        Assert.Throws<InvalidOperationException>(() => stack.Push(disposable));
    }

    private sealed class MockDisposable : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    private sealed class MockAsyncDisposable : IAsyncDisposable
    {
        public bool Disposed { get; private set; }
        public int DisposeCount { get; private set; }
        public bool ShouldThrow { get; init; }
        public Action? DisposeAction { get; set; }

        public async ValueTask DisposeAsync()
        {
            Disposed = true;
            DisposeCount++;
            DisposeAction?.Invoke();

            if (ShouldThrow)
            {
                throw new TestException();
            }

            await ValueTask.CompletedTask;
        }
    }

    private sealed class MockDualDisposable : IAsyncDisposable, IDisposable
    {
        public bool AsyncDisposed { get; private set; }
        public bool SyncDisposed { get; private set; }

        public void Dispose()
        {
            SyncDisposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            AsyncDisposed = true;
            await ValueTask.CompletedTask;
        }
    }

    private sealed class TestException : Exception { }
}
