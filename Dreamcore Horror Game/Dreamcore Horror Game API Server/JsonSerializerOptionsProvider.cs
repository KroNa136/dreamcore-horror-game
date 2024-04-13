using System.Text.Json;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer;

public static class JsonSerializerOptionsProvider
{
    public static JsonSerializerOptions New => new();
    public static JsonSerializerOptions Shared => s_sharedOptions;

    private static readonly JsonSerializerOptions s_sharedOptions;

    static JsonSerializerOptionsProvider()
    {
        s_sharedOptions = new()
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

        s_sharedOptions.Converters.Add(new IpAddressConverter());
    }
}
