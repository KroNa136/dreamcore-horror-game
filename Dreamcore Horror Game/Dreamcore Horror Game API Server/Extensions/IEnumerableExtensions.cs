namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> collection)
        => !collection.Any();

    public static bool IsNotEmpty<T>(this IEnumerable<T> collection)
        => collection.Any();

    public static async Task<T?> FirstOrDefaultAsync<T>(this IEnumerable<T> collection, Func<T, Task<bool>> predicate)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (T element in collection)
            if (await predicate(element))
                return element;

        return default;
    }
}
