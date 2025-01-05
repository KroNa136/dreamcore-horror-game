using System.Text.Json;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameStatisticsServer.Services;

public class JsonSerializerOptionsProvider : IJsonSerializerOptionsProvider
{
    public JsonSerializerOptions New => new();
    public JsonSerializerOptions Default => _defaultOptions;

    private readonly JsonSerializerOptions _defaultOptions;

    public JsonSerializerOptionsProvider()
    {
        _defaultOptions = new()
        {
            AllowTrailingCommas = true,
            DefaultBufferSize = 2048,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,
            MaxDepth = 0,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            WriteIndented = true
        };
    }
}
