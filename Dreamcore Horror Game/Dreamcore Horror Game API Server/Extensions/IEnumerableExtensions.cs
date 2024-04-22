namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any() is false;

    public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any();

    public static IEnumerable<TSource> Paginate<TSource>(this IEnumerable<TSource> source, int page, int showBy)
        => page is > 0 && showBy is > 0
            ? source.Skip(showBy * (page - 1)).Take(showBy)
            : source;

    public static async Task<TSource?> FirstOrDefaultWithAsyncPredicate<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicateTask)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (predicateTask is null)
            throw new ArgumentNullException(nameof(predicateTask));

        foreach (var element in source)
            if (await predicateTask(element))
                return element;

        return default;
    }
}
