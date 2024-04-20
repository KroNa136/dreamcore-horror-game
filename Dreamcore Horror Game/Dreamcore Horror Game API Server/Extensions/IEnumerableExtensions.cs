namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        => !source.Any();

    public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any();

    public static async Task<TSource?> FirstOrDefaultAsync<TSource>(this ParallelQuery<TSource> source, Func<TSource, Task<bool>> predicateTask)
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
