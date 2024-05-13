namespace DreamcoreHorrorGameApiServer.Models;

public record CollectionWithPageCount<TSource>(IEnumerable<TSource> Items, long PageCount);
