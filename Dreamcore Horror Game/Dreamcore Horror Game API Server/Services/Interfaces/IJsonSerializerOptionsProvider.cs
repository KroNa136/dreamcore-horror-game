using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Services;

public interface IJsonSerializerOptionsProvider
{
    public JsonSerializerOptions New => new();
    public JsonSerializerOptions Default { get; }
}
