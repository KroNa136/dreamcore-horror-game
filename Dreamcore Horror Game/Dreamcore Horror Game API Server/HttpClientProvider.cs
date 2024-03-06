namespace Dreamcore_Horror_Game_API_Server
{
    public static class HttpClientProvider
    {
        public static HttpClient New => new();
        public static HttpClient Shared { get; } = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }
}
