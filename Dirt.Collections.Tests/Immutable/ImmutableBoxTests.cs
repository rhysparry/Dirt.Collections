using Dirt.Collections.Immutable;

namespace Dirt.Collections.Tests.Immutable;

public class ImmutableBoxTests
{
    [Fact]
    public void ToImmutableBoxOnAnEmptyBoxReturnsAnEmptyImmutableBox()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();

        // Act
        var immutableBox = emptyBox.ToImmutableBox();

        // Assert
        Assert.True(immutableBox.IsEmpty);
    }

    [Fact]
    public void ToImmutableBoxOnAnEmptyBoxThrowsWhenAccessingContents()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var immutableBox = emptyBox.ToImmutableBox();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => immutableBox.Contents);
    }

    [Fact]
    public void ToImmutableBoxOnAnEmptyBoxReturnsCountOfZero()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();

        // Act
        var immutableBox = emptyBox.ToImmutableBox();

        // Assert
        Assert.Empty(immutableBox);
    }

    [Fact]
    public void ToImmutableBoxOnAnEmptyBoxEnumeratesNoItems()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();

        // Act
        var immutableBox = emptyBox.ToImmutableBox();
        var items = immutableBox.ToList();

        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public void ToImmutableBoxOnANonEmptyBoxReturnsANonEmptyImmutableBox()
    {
        // Arrange
        var box = new Box<int>(42);

        // Act
        var immutableBox = box.ToImmutableBox();

        // Assert
        Assert.False(immutableBox.IsEmpty);
        Assert.Single(immutableBox);
        Assert.Equal(42, immutableBox.Contents);
    }

    [Fact]
    public void ToImmutableBoxOnANonEmptyBoxEnumeratesOneItem()
    {
        // Arrange
        var box = new Box<int>(42);

        // Act
        var immutableBox = box.ToImmutableBox();
        var items = immutableBox.ToList();

        // Assert
        Assert.Single(items);
        Assert.Equal(42, items[0]);
    }

    [Fact]
    public void ToImmutableBoxOnANonEmptyBoxSupportsIndexing()
    {
        // Arrange
        var box = new Box<int>(42);

        // Act
        var immutableBox = box.ToImmutableBox();

        // Assert
        Assert.Equal(42, immutableBox[0]);
    }

    [Fact]
    public void ToImmutableBoxOnANonEmptyBoxThrowsOnInvalidIndex()
    {
        // Arrange
        var box = new Box<int>(42);
        var immutableBox = box.ToImmutableBox();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => immutableBox[1]);
    }

    [Fact]
    public void ToImmutableBoxOnAnAlreadyImmutableBoxReturnsTheSameInstance()
    {
        // Arrange
        var box = new Box<int>(42);
        var immutableBox = box.ToImmutableBox();

        // Act
        var result = immutableBox.ToImmutableBox();

        // Assert
        Assert.Same(immutableBox, result);
    }

    [Fact]
    public void ToImmutableBoxOnAnEmptyBoxReturnedFromAnotherImmutableBoxReturnsTheSameInstance()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var immutableBox = emptyBox.ToImmutableBox();

        // Act
        var result = immutableBox.ToImmutableBox();

        // Assert
        Assert.Same(immutableBox, result);
    }

    [Fact]
    public void ToImmutableBoxPreservesContentsAcrossMultipleConversions()
    {
        // Arrange
        const string originalValue = "TestValue";
        var box = new Box<string>(originalValue);

        // Act
        var immutableBox1 = box.ToImmutableBox();
        var immutableBox2 = immutableBox1.ToImmutableBox();

        // Assert
        Assert.Equal(originalValue, immutableBox1.Contents);
        Assert.Equal(originalValue, immutableBox2.Contents);
        Assert.Same(immutableBox1, immutableBox2);
    }

    [Fact]
    public void ToImmutableBoxWorksWithDifferentTypes()
    {
        // Arrange
        var boxOfString = new Box<string>("Test");
        var boxOfInt = new Box<int>(123);
        var boxOfDouble = new Box<double>(45.67);

        // Act
        var immutableString = boxOfString.ToImmutableBox();
        var immutableInt = boxOfInt.ToImmutableBox();
        var immutableDouble = boxOfDouble.ToImmutableBox();

        // Assert
        Assert.Equal("Test", immutableString.Contents);
        Assert.Equal(123, immutableInt.Contents);
        Assert.Equal(45.67, immutableDouble.Contents);
    }

    [Fact]
    public void ToImmutableBoxEmptyBoxThrowsOnNegativeIndex()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var immutableBox = emptyBox.ToImmutableBox();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => immutableBox[-1]);
    }

    [Fact]
    public void ToImmutableBoxNonEmptyBoxThrowsOnNegativeIndex()
    {
        // Arrange
        var box = new Box<int>(42);
        var immutableBox = box.ToImmutableBox();

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => immutableBox[-1]);
    }

    [Fact]
    public void ToImmutableBoxEmptyBoxEnumeratorBehavior()
    {
        // Arrange
        var emptyBox = Empty.Box<int>();
        var immutableBox = emptyBox.ToImmutableBox();

        // Act
        using var enumerator = immutableBox.GetEnumerator();

        // Assert
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void ToImmutableBoxNonEmptyBoxEnumeratorBehavior()
    {
        // Arrange
        var box = new Box<int>(42);
        var immutableBox = box.ToImmutableBox();

        // Act
        using var enumerator = immutableBox.GetEnumerator();
        var hasItems = enumerator.MoveNext();
        var firstItem = enumerator.Current;
        var hasMoreItems = enumerator.MoveNext();

        // Assert
        Assert.True(hasItems);
        Assert.Equal(42, firstItem);
        Assert.False(hasMoreItems);
    }
}
