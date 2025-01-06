namespace DreamcoreHorrorGameEmailServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any() is false;

    public static bool IsNotEmpty<TSource>(this IEnumerable<TSource> source)
        => source.Any();

    public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in source)
            action(item);

        return source;
    }
}
