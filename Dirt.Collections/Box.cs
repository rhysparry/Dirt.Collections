using System.Diagnostics.CodeAnalysis;

namespace Dirt.Collections;

/// <summary>
/// A Box provides a container for a single value. The box can also be empty.
/// </summary>
/// <typeparam name="T">The type of the value stored in the box.</typeparam>
public sealed class Box<T> : IBox<T>, IReadOnlyBox<T>
    where T : notnull
{
    /// <summary>
    /// Creates a box with the given initial contents.
    /// </summary>
    public Box(T value)
    {
        SetContents(value);
    }

    /// <summary>
    /// Creates an empty box.
    /// </summary>
    public Box() { }

    /// <summary>
    /// Returns true if the box is empty.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Contents))]
    public bool IsEmpty { get; private set; } = true;

    /// <summary>
    /// Returns the current contents of the box. If the box is empty, an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the box is empty</exception>
    public T? Contents
    {
        get => IsEmpty ? throw new InvalidOperationException("Box is empty.") : field;
        private set;
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Contents))]
    public void SetContents(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Contents = value;
        IsEmpty = false;
    }

    /// <inheritdoc />
    public int Count => IsEmpty ? 0 : 1;

    /// <inheritdoc />
    public T this[int index]
    {
        get
        {
            if (IsEmpty || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Contents;
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        if (!IsEmpty)
        {
            yield return Contents;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Static helpers for working with boxes.
/// </summary>
public static class Box
{
    /// <summary>
    /// Create a box from a nullable value. If the value is null, an empty box is returned.
    /// </summary>
    /// <param name="value">The value to store in the box.</param>
    /// <typeparam name="T">The type of the contents of the box.</typeparam>
    /// <returns>A box containing the value, or an empty box if the value was null.</returns>
    public static IBox<T> FromNullable<T>(T? value)
        where T : notnull
    {
        return value is null ? new Box<T>() : new Box<T>(value);
    }

    /// <summary>
    /// Create a box from a nullable value. If the value is null, an empty box is returned.
    /// </summary>
    /// <param name="value">The value to store in the box.</param>
    /// <typeparam name="T">The type of the contents of the box.</typeparam>
    /// <returns>A box containing the value, or an empty box if the value was null.</returns>
    public static IBox<T> FromNullable<T>(T? value)
        where T : struct
    {
        return value.HasValue ? new Box<T>(value.Value) : new Box<T>();
    }

    /// <summary>
    /// Puts the given value into a new box.
    /// </summary>
    /// <param name="value">The value to box.</param>
    /// <typeparam name="T">The type of the item in the box.</typeparam>
    /// <returns>A box containing the value</returns>
    public static IBox<T> IntoBox<T>(this T value)
        where T : notnull
    {
        return new Box<T>(value);
    }
}
