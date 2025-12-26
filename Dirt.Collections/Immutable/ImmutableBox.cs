using System.Collections;

namespace Dirt.Collections.Immutable;

/// <summary>
/// Helper extensions for operations related to immutable boxes.
/// </summary>
public static class ImmutableBox
{
    /// <summary>
    /// Returns an immutable version of the given read-only box.
    ///
    /// If the box is already immutable, it is returned as-is.
    /// </summary>
    /// <param name="box">The box to base the new box from</param>
    /// <typeparam name="T">The type of contents in the box</typeparam>
    /// <returns>An immutable version of the box</returns>
    public static IImmutableBox<T> ToImmutableBox<T>(this IReadOnlyBox<T> box)
        where T : notnull
    {
        if (box is IImmutableBox<T> immutableBox)
        {
            return immutableBox;
        }

        if (box.IsEmpty)
        {
            return new EmptyBox<T>();
        }

        return new PopulatedBox<T>(box.Contents);
    }

    private sealed class EmptyBox<T> : IImmutableBox<T>
        where T : notnull
    {
        public bool IsEmpty => true;

        public T Contents => throw new InvalidOperationException("Box is empty.");

        public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => 0;

        public T this[int index] => throw new IndexOutOfRangeException();
    }

    private sealed class PopulatedBox<T>(T contents) : IImmutableBox<T>
        where T : notnull
    {
        public bool IsEmpty => false;

        public T Contents => contents;

        public IEnumerator<T> GetEnumerator()
        {
            yield return contents;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => 1;

        public T this[int index] => index == 0 ? contents : throw new IndexOutOfRangeException();
    }
}
