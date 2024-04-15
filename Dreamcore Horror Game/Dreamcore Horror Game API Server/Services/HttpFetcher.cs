using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace DreamcoreHorrorGameApiServer.Services;

public class HttpFetcher : IHttpFetcher
{
    private readonly UriBuilder _uriBuilder;
    private readonly HttpClient _httpClient;

    public HttpFetcher()
    {
        _uriBuilder = new();

        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Add(CorsHeaders.ApiServer, string.Empty);
    }

    public async Task<HttpResponseMessage> GetAsync(string host, int port, string path)
    {
        _uriBuilder.Host = host;
        _uriBuilder.Port = port;
        _uriBuilder.Path = path;

        return await _httpClient.GetAsync(_uriBuilder.Uri);
    }

    public async Task<HttpResponseMessage> PostAsync(string host, int port, string path, HttpContent? content)
    {
        _uriBuilder.Host = host;
        _uriBuilder.Port = port;
        _uriBuilder.Path = path;

        return await _httpClient.PostAsync(_uriBuilder.Uri, content);
    }
}
