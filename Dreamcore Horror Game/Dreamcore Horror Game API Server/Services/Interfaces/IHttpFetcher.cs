namespace DreamcoreHorrorGameApiServer.Services;

public interface IHttpFetcher
{
    public Task<HttpResponseMessage> GetAsync(string host, int port, string path);
    public Task<HttpResponseMessage> PostAsync(string host, int port, string path, HttpContent? content);
}
