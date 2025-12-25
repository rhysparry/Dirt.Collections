namespace Dirt.Collections;

/// <summary>
/// Provides helpers for creating empty collections.
/// </summary>
public static class Empty
{
    /// <summary>
    /// Returns an empty box of the given type.
    /// </summary>
    /// <typeparam name="T">The type of values in the box.</typeparam>
    /// <returns>A new empty box</returns>
    public static Box<T> Box<T>()
        where T : notnull => new();
}
