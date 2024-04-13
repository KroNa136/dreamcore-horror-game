namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> collection)
        => !collection.Any();

    public static bool IsNotEmpty<T>(this IEnumerable<T> collection)
        => collection.Any();
}
