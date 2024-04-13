namespace DreamcoreHorrorGameApiServer;

public static class HttpClientProvider
{
    public static HttpClient New => new();
    public static HttpClient Shared => s_sharedClient;

    private static readonly HttpClient s_sharedClient = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };
}
