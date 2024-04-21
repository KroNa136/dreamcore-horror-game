namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IQueryableExtensions
{
    public static ParallelQuery<TSource> AsForceParallel<TSource>(this IQueryable<TSource> source)
        => source
            .AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .WithMergeOptions(ParallelMergeOptions.FullyBuffered);

    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int showBy)
        => page is > 0 && showBy is > 0
            ? source.Skip(showBy * (page - 1)).Take(showBy)
            : source;
}
