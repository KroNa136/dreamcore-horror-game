using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Services;
using System.Net;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class ObjectExtensions
{
    private static readonly Dictionary<Type, Func<string, object>> s_supportedParseTypes = new()
    {
        { typeof(bool), value => bool.Parse(value) },
        { typeof(bool?), value => bool.Parse(value) },
        { typeof(char), value => value.Length is 1 ? value.First() : throw new FormatException() },
        { typeof(char?), value => value.Length is 1 ? value.First() : throw new FormatException() },
        { typeof(string), value => value },
        { typeof(byte), value => byte.Parse(value) },
        { typeof(byte?), value => byte.Parse(value) },
        { typeof(short), value => short.Parse(value) },
        { typeof(short?), value => short.Parse(value) },
        { typeof(int), value => int.Parse(value) },
        { typeof(int?), value => int.Parse(value) },
        { typeof(long), value => long.Parse(value) },
        { typeof(long?), value => long.Parse(value) },
        { typeof(Half), value => Half.Parse(value) },
        { typeof(Half?), value => Half.Parse(value) },
        { typeof(float), value => float.Parse(value) },
        { typeof(float?), value => float.Parse(value) },
        { typeof(double), value => double.Parse(value) },
        { typeof(double?), value => double.Parse(value) },
        { typeof(decimal), value => decimal.Parse(value) },
        { typeof(decimal?), value => decimal.Parse(value) },
        { typeof(DateTime), value => DateTime.Parse(value) },
        { typeof(DateTime?), value => DateTime.Parse(value) },
        { typeof(DateOnly), value => DateOnly.Parse(value) },
        { typeof(DateOnly?), value => DateOnly.Parse(value) },
        { typeof(TimeOnly), value => TimeOnly.Parse(value) },
        { typeof(TimeOnly?), value => TimeOnly.Parse(value) },
        { typeof(Guid), value => Guid.Parse(value) },
        { typeof(Guid?), value => Guid.Parse(value) },
        { typeof(IPAddress), IPAddress.Parse },
    };

    public static TResult ParseToType<TResult>(this object value)
        => (TResult) Parse(value, typeof(TResult));

    public static TResult? ParseToTypeOrDefault<TResult>(this object value)
    {
        try
        {
            return (TResult) Parse(value, typeof(TResult));
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            return default;
        }
    }

    public static bool TryParseToType<TResult>(this object value, out TResult? objOfType)
    {
        try
        {
            objOfType = (TResult) Parse(value, typeof(TResult));
            return true;
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            objOfType = default;
            return false;
        }
    }

    public static bool CanBeParsedToType<TResult>(this object value)
        => value.ParseToTypeOrDefault<TResult>() is not null;

    public static bool CannotBeParsedToType<TResult>(this object value)
        => value.ParseToTypeOrDefault<TResult>() is null;

    public static object? ParseToObjectOfType(this object value, Type type)
        => Parse(value, type);

    public static object? ParseToObjectOfTypeOrDefault(this object value, Type type)
    {
        try
        {
            return Parse(value, type);
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            return default;
        }
    }

    public static bool TryParseToObjectOfType(this object value, Type type, out object? objOfType)
    {
        try
        {
            objOfType = Parse(value, type);
            return true;
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            objOfType = null;
            return false;
        }
    }

    public static bool CanBeParsedToType(this object value, Type type)
        => value.ParseToObjectOfTypeOrDefault(type) is not null;

    public static bool CannotBeParsedToType(this object value, Type type)
        => value.ParseToObjectOfTypeOrDefault(type) is null;

    private static object Parse(object value, Type targetType)
    {
        string? stringValue = value.ToString();

        return stringValue is null
            ? Convert.ChangeType(value, targetType)
            : s_supportedParseTypes.TryGetValue(targetType, out var parse)
            ? parse(stringValue)
            : targetType.GetInterfaces().Contains(typeof(IDatabaseEntity))
            ? ParseFromJson(stringValue, targetType)
            : throw new NotSupportedException();
    }

    private static object ParseFromJson(string json, Type targetType)
        => JsonSerializer.Deserialize(json, targetType, new JsonSerializerOptionsProvider().Default) ?? string.Empty;
}
