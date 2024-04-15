using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class JsonOptionsExtensions
{
    public static void ConfigureFrom(this JsonOptions options, JsonSerializerOptions fromOptions)
    {
        options.JsonSerializerOptions.AllowTrailingCommas = fromOptions.AllowTrailingCommas;

        if (fromOptions.Converters is not null && fromOptions.Converters.IsNotEmpty())
            foreach (var converter in fromOptions.Converters)
                options.JsonSerializerOptions.Converters.Add(converter);

        options.JsonSerializerOptions.DefaultBufferSize = fromOptions.DefaultBufferSize;
        options.JsonSerializerOptions.DefaultIgnoreCondition = fromOptions.DefaultIgnoreCondition;

        if (fromOptions.DictionaryKeyPolicy is not null)
            options.JsonSerializerOptions.DictionaryKeyPolicy = fromOptions.DictionaryKeyPolicy;

        if (fromOptions.Encoder is not null)
            options.JsonSerializerOptions.Encoder = fromOptions.Encoder;

        options.JsonSerializerOptions.IgnoreReadOnlyFields = fromOptions.IgnoreReadOnlyFields;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = fromOptions.IgnoreReadOnlyProperties;
        options.JsonSerializerOptions.IncludeFields = fromOptions.IncludeFields;
        options.JsonSerializerOptions.MaxDepth = fromOptions.MaxDepth;
        options.JsonSerializerOptions.NumberHandling = fromOptions.NumberHandling;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = fromOptions.PropertyNameCaseInsensitive;

        if (fromOptions.PropertyNamingPolicy is not null)
            options.JsonSerializerOptions.PropertyNamingPolicy = fromOptions.PropertyNamingPolicy;

        options.JsonSerializerOptions.ReadCommentHandling = fromOptions.ReadCommentHandling;

        if (fromOptions.ReferenceHandler is not null)
            options.JsonSerializerOptions.ReferenceHandler = fromOptions.ReferenceHandler;
        
        if (fromOptions.TypeInfoResolver is not null)
            options.JsonSerializerOptions.TypeInfoResolver = fromOptions.TypeInfoResolver;

        options.JsonSerializerOptions.UnknownTypeHandling = fromOptions.UnknownTypeHandling;
        options.JsonSerializerOptions.WriteIndented = fromOptions.WriteIndented;
    }
}
