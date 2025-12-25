namespace Dirt.Collections.Tests;

public class ReadOnlyBoxTests
{
    [Fact]
    public void MappingAnEmptyBoxReturnsAnEmptyBoxAndDoesNotInvokeMapper()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var mapperInvoked = false;

        // Act
        var resultBox = emptyBox.Map(value =>
        {
            mapperInvoked = true;
            return value.ToString();
        });

        // Assert
        Assert.True(resultBox.IsEmpty);
        Assert.False(mapperInvoked);
    }

    [Fact]
    public void MappingANonEmptyBoxInvokesMapperAndReturnsMappedValue()
    {
        // Arrange
        var box = new Box<int>(42);
        var mapperInvoked = false;

        // Act
        var resultBox = box.Map(value =>
        {
            mapperInvoked = true;
            return value.ToString();
        });

        // Assert
        Assert.False(resultBox.IsEmpty);
        Assert.Equal("42", resultBox.Contents);
        Assert.True(mapperInvoked);
    }

    [Fact]
    public void MapBoxOnAnEmptyBoxReturnsAnEmptyBoxAndDoesNotInvokeMapper()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var mapperInvoked = false;

        // Act
        var resultBox = emptyBox.MapBox(value =>
        {
            mapperInvoked = true;
            return new Box<string>(value.ToString());
        });

        // Assert
        Assert.True(resultBox.IsEmpty);
        Assert.False(mapperInvoked);
    }

    [Fact]
    public void MapBoxOnANonEmptyBoxInvokesMapperAndReturnsResultBox()
    {
        // Arrange
        var box = new Box<int>(42);
        var mapperInvoked = false;

        // Act
        var resultBox = box.MapBox(value =>
        {
            mapperInvoked = true;
            return new Box<string>(value.ToString());
        });

        // Assert
        Assert.False(resultBox.IsEmpty);
        Assert.Equal("42", resultBox.Contents);
        Assert.True(mapperInvoked);
    }

    [Fact]
    public void MapBoxOnANonEmptyBoxReturnsEmptyBoxWhenMapperReturnsEmpty()
    {
        // Arrange
        var box = new Box<int>(42);

        // Act
        var resultBox = box.MapBox(_ => Empty.Box<string>());

        // Assert
        Assert.True(resultBox.IsEmpty);
    }

    [Fact]
    public async Task MapAsyncOnAnEmptyBoxReturnsAnEmptyBoxAndDoesNotInvokeMapper()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var mapperInvoked = false;

        // Act
        var resultBox = await emptyBox.MapAsync(
            async (value, ct) =>
            {
                mapperInvoked = true;
                await Task.Delay(0, ct);
                return value.ToString();
            },
            CancellationToken.None
        );

        // Assert
        Assert.True(resultBox.IsEmpty);
        Assert.False(mapperInvoked);
    }

    [Fact]
    public async Task MapAsyncOnANonEmptyBoxInvokesMapperAndReturnsMappedValue()
    {
        // Arrange
        var box = new Box<int>(42);
        var mapperInvoked = false;

        // Act
        var resultBox = await box.MapAsync(
            async (value, ct) =>
            {
                mapperInvoked = true;
                await Task.CompletedTask;
                ct.ThrowIfCancellationRequested();
                return value.ToString();
            },
            CancellationToken.None
        );

        // Assert
        Assert.False(resultBox.IsEmpty);
        Assert.Equal("42", resultBox.Contents);
        Assert.True(mapperInvoked);
    }

    [Fact]
    public async Task MapAsyncRespectsCancellationToken()
    {
        // Arrange
        var box = new Box<int>(42);
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await box.MapAsync(
                async (value, ct) =>
                {
                    await Task.CompletedTask;
                    ct.ThrowIfCancellationRequested();
                    return value.ToString();
                },
                cancellationTokenSource.Token
            );
        });
    }

    [Fact]
    public async Task MapBoxAsyncOnAnEmptyBoxReturnsAnEmptyBoxAndDoesNotInvokeMapper()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var mapperInvoked = false;

        // Act
        var resultBox = await emptyBox.MapBoxAsync(
            async (value, ct) =>
            {
                mapperInvoked = true;
                await Task.CompletedTask;
                ct.ThrowIfCancellationRequested();
                return new Box<string>(value.ToString());
            },
            CancellationToken.None
        );

        // Assert
        Assert.True(resultBox.IsEmpty);
        Assert.False(mapperInvoked);
    }

    [Fact]
    public async Task MapBoxAsyncOnANonEmptyBoxInvokesMapperAndReturnsResultBox()
    {
        // Arrange
        var box = new Box<int>(42);
        var mapperInvoked = false;

        // Act
        var resultBox = await box.MapBoxAsync(
            async (value, ct) =>
            {
                mapperInvoked = true;
                await Task.CompletedTask;
                ct.ThrowIfCancellationRequested();
                return new Box<string>(value.ToString());
            },
            CancellationToken.None
        );

        // Assert
        Assert.False(resultBox.IsEmpty);
        Assert.Equal("42", resultBox.Contents);
        Assert.True(mapperInvoked);
    }

    [Fact]
    public async Task MapBoxAsyncOnANonEmptyBoxReturnsEmptyBoxWhenMapperReturnsEmpty()
    {
        // Arrange
        var box = new Box<int>(42);

        // Act
        var resultBox = await box.MapBoxAsync(
            async (_, ct) =>
            {
                await Task.CompletedTask;
                ct.ThrowIfCancellationRequested();
                return Empty.Box<string>();
            },
            CancellationToken.None
        );

        // Assert
        Assert.True(resultBox.IsEmpty);
    }

    [Fact]
    public async Task MapBoxAsyncRespectsCancellationToken()
    {
        // Arrange
        var box = new Box<int>(42);
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await box.MapBoxAsync(
                async (value, ct) =>
                {
                    await Task.CompletedTask;
                    ct.ThrowIfCancellationRequested();
                    return new Box<string>(value.ToString());
                },
                cancellationTokenSource.Token
            );
        });
    }
}
