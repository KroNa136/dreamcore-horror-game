using System.Text.Json;

namespace DreamcoreHorrorGameEmailServer.Services;

public interface IJsonSerializerOptionsProvider
{
    public JsonSerializerOptions New { get; }
    public JsonSerializerOptions Default { get; }
}
