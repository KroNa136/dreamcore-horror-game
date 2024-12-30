using DreamcoreHorrorGameApiServer.ConstantValues;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace DreamcoreHorrorGameApiServer.Services;

public class HttpFetcher : IHttpFetcher
{
    private readonly ILogger<HttpFetcher> _logger;

    private readonly UriBuilder _uriBuilder;
    private readonly HttpClient _httpClient;

    public HttpFetcher(ILogger<HttpFetcher> logger)
    {
        _logger = logger;

        _uriBuilder = new();

        _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Add(CorsHeaders.ApiServer, CorsHeaders.RequiredHeaderValue);
    }

    public async Task<HttpResponseMessage?> GetAsync(string host, int port, string path)
    {
        _uriBuilder.Host = host;
        _uriBuilder.Port = port;
        _uriBuilder.Path = path;

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_uriBuilder.Uri);
            return response;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogInformation
            (
                eventId: new EventId("GetAsync".GetHashCode() + ex.GetType().GetHashCode()),
                message: "A task cancellation was initiated while executing a GET request to {Uri}.", _uriBuilder.Uri
            );

            return null;
        }
    }

    public async Task<HttpResponseMessage?> PostAsync(string host, int port, string path, HttpContent? content)
    {
        _uriBuilder.Host = host;
        _uriBuilder.Port = port;
        _uriBuilder.Path = path;

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync(_uriBuilder.Uri, content);
            return response;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogInformation
            (
                eventId: new EventId("PostAsync".GetHashCode() + ex.GetType().GetHashCode()),
                message: "A task cancellation was initiated while executing a POST request to {Uri}.", _uriBuilder.Uri
            );

            return null;
        }
    }
}
