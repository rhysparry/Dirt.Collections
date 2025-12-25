using System.Diagnostics.CodeAnalysis;

namespace Dirt.Collections;

/// <summary>
/// A Box provides a container for a single value. The box can also be empty.
///
/// The contents of the box can be changed.
/// </summary>
/// <typeparam name="T">The type of the value stored in the box</typeparam>
public interface IBox<T> : IReadOnlyList<T>
    where T : notnull
{
    /// <summary>
    /// Sets the contents of the box to the given value. If the box has a current value,
    /// it will be replaced.
    /// </summary>
    /// <param name="value">The value to set as the contents of the box.</param>
    /// <remarks>
    /// An <see cref="ArgumentNullException"/> will be thrown if <paramref name="value"/> is null.
    /// </remarks>
    [MemberNotNull(nameof(Contents))]
    void SetContents(T value);

    /// <summary>
    /// Returns true if the box is empty.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Contents))]
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the current contents of the box. If the box is empty, an <see cref="InvalidOperationException"/> is thrown.
    /// </summary>
    /// <remarks>
    /// This property has a nullable annotation to help ensure users check <see cref="IsEmpty"/> before accessing the contents.
    /// </remarks>
    T? Contents { get; }
}
