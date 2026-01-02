using System.Collections;
using Dirt.Collections.Immutable;

namespace Dirt.Collections.Tests;

public class BoxTests
{
    [Fact]
    public void DefaultConstructor_CreatesEmptyBox()
    {
        var box = new Box<string>();
        Assert.True(box.IsEmpty);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = box.Contents;
        });
    }

    [Fact]
    public void Constructor_WithValue_CreatesBoxWithContents()
    {
        var box = new Box<string>("value");
        Assert.False(box.IsEmpty);
        Assert.Equal("value", box.Contents);
    }

    [Fact]
    public void Constructor_WithNullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Box<string>(null!);
        });
    }

    [Fact]
    public void EmptyBox_BehavesAsEmptyReadOnlyList()
    {
        var box = Empty.Box<string>();
        Assert.True(box.IsEmpty);
        Assert.Empty(box);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = box[0];
        });

        using var enumerator = box.GetEnumerator();
        Assert.False(enumerator.MoveNext());
        var nonGenericEnumerator = ((IEnumerable)box).GetEnumerator();
        using var nonGenericEnumeratorDisposable = nonGenericEnumerator as IDisposable;
        Assert.False(nonGenericEnumerator.MoveNext());
    }

    [Fact]
    public void PopulatedBox_BehavesAsSingleElementReadOnlyList()
    {
        var box = new Box<string>("value");
        Assert.False(box.IsEmpty);
        Assert.Single(box);
        Assert.Equal("value", box[0]);

        using var enumerator = box.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal("value", enumerator.Current);
        Assert.False(enumerator.MoveNext());

        var nonGenericEnumerator = ((IEnumerable)box).GetEnumerator();
        using var nonGenericEnumeratorDisposable = nonGenericEnumerator as IDisposable;
        Assert.True(nonGenericEnumerator.MoveNext());
        Assert.Equal("value", nonGenericEnumerator.Current);
        Assert.False(nonGenericEnumerator.MoveNext());
    }

    [Fact]
    public void BoxFromNullable_WithReferenceType_NullValue_CreatesEmptyBox()
    {
        string? value = null;
        var box = Box.FromNullable(value);
        Assert.True(box.IsEmpty);
    }

    [Fact]
    public void BoxFromNullable_WithReferenceType_NonNullValue_CreatesBoxWithContents()
    {
        string? value = null;
        Assert.Null(value);
        value = "value";
        var box = Box.FromNullable(value);
        Assert.False(box.IsEmpty);
        Assert.Equal(value, box.Contents);
    }

    [Fact]
    public void BoxFromNullable_WithValueType_NullValue_CreatesEmptyBox()
    {
        int? value = null;
        var box = Box.FromNullable(value);
        Assert.True(box.IsEmpty);
    }

    [Fact]
    public void BoxFromNullable_WithValueType_NonNullValue_CreatesBoxWithContents()
    {
        int? value = 42;
        var box = Box.FromNullable(value);
        Assert.False(box.IsEmpty);
        Assert.Equal(42, box.Contents);
    }

    [Fact]
    public void ValueIntoBox_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            string value = null!;
            value.IntoBox();
        });
    }

    [Fact]
    public void ValueIntoBox_WithNonNull_CreatesBoxWithContents()
    {
        const string value = "value";
        var box = value.IntoBox();
        Assert.False(box.IsEmpty);
        Assert.Equal(value, box.Contents);
    }

    [Fact]
    public void SetContentsOnEmptyBox_SetsContents()
    {
        var box = Empty.Box<string>();
        box.SetContents("new value");
        Assert.False(box.IsEmpty);
        Assert.Equal("new value", box.Contents);
    }

    [Fact]
    public void SetContentsOnPopulatedBox_UpdatesContents()
    {
        var box = new Box<string>("initial value");
        box.SetContents("updated value");
        Assert.False(box.IsEmpty);
        Assert.Equal("updated value", box.Contents);
    }

    [Fact]
    public void SetContents_WithNull_Throws()
    {
        var box = Empty.Box<string>();
        Assert.Throws<ArgumentNullException>(() =>
        {
            box.SetContents(null!);
        });
    }

    [Fact]
    public void TwoEmptyBoxes_Equal_True()
    {
        var box_one = Empty.Box<string>();
        var box_two = Empty.Box<string>();

        Assert.True(box_one.Equals(box_two));
        Assert.Equal(box_one.GetHashCode(), box_two.GetHashCode());
    }

    [Fact]
    public void SameContents_Equal_True()
    {
        var box_one = new Box<string>("initial value");
        var box_two = new Box<string>("initial value");

        Assert.True(box_one.Equals(box_two));
        Assert.Equal(box_one.GetHashCode(), box_two.GetHashCode());
    }

    [Fact]
    public void DifferentContents_Equal_False()
    {
        var box_one = new Box<string>("initial value");
        var box_two = new Box<string>("initial value 2");

        Assert.False(box_one.Equals(box_two));
        Assert.NotEqual(box_one.GetHashCode(), box_two.GetHashCode());
    }
}
