using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Elfie.Serialization;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any() is false;

    public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any();

    public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (var item in source)
            action(item);

        return source;
    }

    public static IEnumerable<TSource> Paginate<TSource>(this IEnumerable<TSource> source, int page, int showBy)
        => page is > 0 && showBy is > 0
            ? source.Skip(showBy * (page - 1)).Take(showBy)
            : source;

    public static long PageCount<TSource>(this IEnumerable<TSource> source, int showBy)
        => showBy > 0
            ? (long) Math.Ceiling((float) source.Count() / showBy)
            : -1;

    public static CollectionWithPageCount<TSource> WithPageCount<TSource>(this IEnumerable<TSource> entities, long pageCount)
        => new(entities, pageCount);

    public static CollectionWithPageCount<TSource> PaginatedAndWithPageCount<TSource>(this IEnumerable<TSource> source, int page, int showBy)
        => source
            .Paginate(page, showBy)
            .WithPageCount(source.PageCount(showBy));

    public static async Task<TSource?> FirstOrDefaultWithAsyncPredicate<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicateTask)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (predicateTask is null)
            throw new ArgumentNullException(nameof(predicateTask));

        foreach (var item in source)
            if (await predicateTask(item))
                return item;

        return default;
    }
}
