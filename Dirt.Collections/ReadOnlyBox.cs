namespace Dirt.Collections;

/// <summary>
/// Helper extensions for operations on a read-only box.
/// </summary>
public static class ReadOnlyBox
{
    /// <param name="box">The box to map from</param>
    /// <typeparam name="T1">The type of the content being mapped from.</typeparam>
    extension<T1>(IReadOnlyBox<T1> box)
        where T1 : notnull
    {
        /// <summary>
        /// Maps the contents of the given box using the provided mapper function.
        ///
        /// Empty boxes are always mapped to empty boxes.
        /// </summary>
        /// <param name="mapper">The mapping function to apply to the contents of the box</param>
        /// <typeparam name="T2">The type of the content being mapped to.</typeparam>
        /// <returns>The result of the mapper in a box, if the initial box was not empty, otherwise an empty box.</returns>
        public Box<T2> Map<T2>(Func<T1, T2> mapper)
            where T2 : notnull
        {
            if (box.IsEmpty)
            {
                return new Box<T2>();
            }

            return new Box<T2>(mapper(box.Contents));
        }

        /// <summary>
        /// Maps the contents of the given box using the provided mapper function that returns a box.
        ///
        /// This allows for mapping functions that may also return empty boxes.
        /// </summary>
        /// <param name="mapper">The mapping function to apply to the contents of the box</param>
        /// <typeparam name="T2">The type of the content being mapped to.</typeparam>
        /// <returns>The result of the mapper, if the initial box was not empty, otherwise an empty box.</returns>
        public Box<T2> MapBox<T2>(Func<T1, Box<T2>> mapper)
            where T2 : notnull
        {
            if (box.IsEmpty)
            {
                return new Box<T2>();
            }

            return mapper(box.Contents);
        }

        /// <summary>
        /// Maps the contents of the given box using the provided asynchronous mapper function.
        /// </summary>
        /// <param name="mapper">The mapper function to apply to the contents of the box</param>
        /// <param name="cancellationToken">Cancellation Token to be used in the mapper function.</param>
        /// <typeparam name="T2">The type of the content being mapped to.</typeparam>
        /// <returns>The result of the mapper, if the initial box was not empty, otherwise an empty box.</returns>
        public async Task<Box<T2>> MapAsync<T2>(
            Func<T1, CancellationToken, Task<T2>> mapper,
            CancellationToken cancellationToken
        )
            where T2 : notnull
        {
            if (box.IsEmpty)
            {
                return new Box<T2>();
            }

            var result = await mapper(box.Contents, cancellationToken);
            return new Box<T2>(result);
        }

        /// <summary>
        /// Maps the contents of the given box using the provided asynchronous mapper function that returns a box.
        ///
        /// This allows for mapping functions that may also return empty boxes.
        /// </summary>
        /// <param name="mapper">The mapper function to apply to the contents of the box</param>
        /// <param name="cancellationToken">Cancellation Token to be used in the mapper function.</param>
        /// <typeparam name="T2">The type of the content being mapped to.</typeparam>
        /// <returns>The result of the mapper, if the initial box was not empty, otherwise an empty box.</returns>
        public async Task<Box<T2>> MapBoxAsync<T2>(
            Func<T1, CancellationToken, Task<Box<T2>>> mapper,
            CancellationToken cancellationToken
        )
            where T2 : notnull
        {
            if (box.IsEmpty)
            {
                return new Box<T2>();
            }

            return await mapper(box.Contents, cancellationToken);
        }
    }
}
