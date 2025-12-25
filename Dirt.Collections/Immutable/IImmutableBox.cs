namespace Dirt.Collections.Immutable;

/// <summary>
/// A marker interface for a Box that cannot have its contents changed.
/// </summary>
/// <typeparam name="T">The type of the box contents.</typeparam>
public interface IImmutableBox<out T> : IReadOnlyBox<T>
    where T : notnull;
