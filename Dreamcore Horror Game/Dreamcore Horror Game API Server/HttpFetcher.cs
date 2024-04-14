using DreamcoreHorrorGameApiServer.ConstantValues;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace DreamcoreHorrorGameApiServer;

public static class HttpFetcher
{
    private static readonly UriBuilder s_uriBuilder;
    private static readonly HttpClient s_httpClient;

    static HttpFetcher()
    {
        s_uriBuilder = new();

        s_httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        s_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        s_httpClient.DefaultRequestHeaders.Add(CorsHeaders.ApiServer, string.Empty);
    }

    public static async Task<HttpResponseMessage> GetAsync(string host, int port, string path)
    {
        s_uriBuilder.Host = host;
        s_uriBuilder.Port = port;
        s_uriBuilder.Path = path;

        return await s_httpClient.GetAsync(s_uriBuilder.Uri);
    }

    public static async Task<HttpResponseMessage> PostAsync(string host, int port, string path, HttpContent? content)
    {
        s_uriBuilder.Host = host;
        s_uriBuilder.Port = port;
        s_uriBuilder.Path = path;

        return await s_httpClient.PostAsync(s_uriBuilder.Uri, content);
    }
}
