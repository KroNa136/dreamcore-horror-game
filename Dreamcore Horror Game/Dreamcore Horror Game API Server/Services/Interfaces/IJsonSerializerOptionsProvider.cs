using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Services;

public interface IJsonSerializerOptionsProvider
{
    public JsonSerializerOptions New { get; }
    public JsonSerializerOptions Default { get; }
}
