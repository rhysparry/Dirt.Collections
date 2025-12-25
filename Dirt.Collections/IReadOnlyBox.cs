using System.Diagnostics.CodeAnalysis;

namespace Dirt.Collections;

/// <summary>
/// A read-only Box provides a container that may or may not have a single value.
/// </summary>
/// <typeparam name="T">The type of the value stored in the box</typeparam>
public interface IReadOnlyBox<out T> : IReadOnlyList<T>
    where T : notnull
{
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
