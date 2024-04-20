namespace DreamcoreHorrorGameApiServer.Extensions;

public static class IQueryableExtensions
{
    public static ParallelQuery<TSource> AsForceParallel<TSource>(this IQueryable<TSource> source)
        => source
            .AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .WithMergeOptions(ParallelMergeOptions.FullyBuffered);
}
