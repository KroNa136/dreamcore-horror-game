namespace DreamcoreHorrorGameApiServer.Extensions;

public static class HttpResponseMessageExtensions
{
    public static bool IsNotSuccessStatusCode(this HttpResponseMessage message)
        => message.IsSuccessStatusCode is false;
}
