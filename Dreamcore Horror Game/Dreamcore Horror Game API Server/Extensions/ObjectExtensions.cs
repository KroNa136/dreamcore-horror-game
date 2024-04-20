using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Services;
using System.Net;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class ObjectExtensions
{
    public static TResult ParseToType<TResult>(this object value)
    {
        string? stringValue = value.ToString();

        object? obj = stringValue is null ? Convert.ChangeType(value, typeof(TResult))
            : typeof(TResult) == typeof(bool) || typeof(TResult) == typeof(bool?) ? bool.Parse(stringValue)
            : typeof(TResult) == typeof(char) || typeof(TResult) == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
            : typeof(TResult) == typeof(string) ? stringValue
            : typeof(TResult) == typeof(byte) || typeof(TResult) == typeof(byte?) ? byte.Parse(stringValue)
            : typeof(TResult) == typeof(short) || typeof(TResult) == typeof(short?) ? short.Parse(stringValue)
            : typeof(TResult) == typeof(int) || typeof(TResult) == typeof(int?) ? int.Parse(stringValue)
            : typeof(TResult) == typeof(long) || typeof(TResult) == typeof(long?) ? long.Parse(stringValue)
            : typeof(TResult) == typeof(Half) || typeof(TResult) == typeof(Half?) ? Half.Parse(stringValue)
            : typeof(TResult) == typeof(float) || typeof(TResult) == typeof(float?) ? float.Parse(stringValue)
            : typeof(TResult) == typeof(double) || typeof(TResult) == typeof(double?) ? double.Parse(stringValue)
            : typeof(TResult) == typeof(decimal) || typeof(TResult) == typeof(decimal?) ? decimal.Parse(stringValue)
            : typeof(TResult) == typeof(DateTime) || typeof(TResult) == typeof(DateTime?) ? DateTime.Parse(stringValue)
            : typeof(TResult) == typeof(DateOnly) || typeof(TResult) == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
            : typeof(TResult) == typeof(TimeOnly) || typeof(TResult) == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
            : typeof(TResult) == typeof(Guid) || typeof(TResult) == typeof(Guid?) ? Guid.Parse(stringValue)
            : typeof(TResult) == typeof(IPAddress) ? IPAddress.Parse(stringValue)
            : typeof(TResult).GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, typeof(TResult))
            : throw new NotSupportedException();

        return (TResult) obj;
    }

    public static TResult? ParseToTypeOrDefault<TResult>(this object value)
    {
        string? stringValue = value.ToString();

        try
        {
            object? obj = stringValue is null ? Convert.ChangeType(value, typeof(TResult))
                : typeof(TResult) == typeof(bool) || typeof(TResult) == typeof(bool?) ? bool.Parse(stringValue)
                : typeof(TResult) == typeof(char) || typeof(TResult) == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
                : typeof(TResult) == typeof(string) ? stringValue
                : typeof(TResult) == typeof(byte) || typeof(TResult) == typeof(byte?) ? byte.Parse(stringValue)
                : typeof(TResult) == typeof(short) || typeof(TResult) == typeof(short?) ? short.Parse(stringValue)
                : typeof(TResult) == typeof(int) || typeof(TResult) == typeof(int?) ? int.Parse(stringValue)
                : typeof(TResult) == typeof(long) || typeof(TResult) == typeof(long?) ? long.Parse(stringValue)
                : typeof(TResult) == typeof(Half) || typeof(TResult) == typeof(Half?) ? Half.Parse(stringValue)
                : typeof(TResult) == typeof(float) || typeof(TResult) == typeof(float?) ? float.Parse(stringValue)
                : typeof(TResult) == typeof(double) || typeof(TResult) == typeof(double?) ? double.Parse(stringValue)
                : typeof(TResult) == typeof(decimal) || typeof(TResult) == typeof(decimal?) ? decimal.Parse(stringValue)
                : typeof(TResult) == typeof(DateTime) || typeof(TResult) == typeof(DateTime?) ? DateTime.Parse(stringValue)
                : typeof(TResult) == typeof(DateOnly) || typeof(TResult) == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
                : typeof(TResult) == typeof(TimeOnly) || typeof(TResult) == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
                : typeof(TResult) == typeof(Guid) || typeof(TResult) == typeof(Guid?) ? Guid.Parse(stringValue)
                : typeof(TResult) == typeof(IPAddress) ? IPAddress.Parse(stringValue)
                : typeof(TResult).GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, typeof(TResult))
                : throw new NotSupportedException();

            return (TResult) obj;
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            return default;
        }
    }

    public static bool TryParseToType<TResult>(this object value, out TResult? objOfType)
    {
        string? stringValue = value.ToString();

        try
        {
            object? obj = stringValue is null ? Convert.ChangeType(value, typeof(TResult))
                : typeof(TResult) == typeof(bool) || typeof(TResult) == typeof(bool?) ? bool.Parse(stringValue)
                : typeof(TResult) == typeof(char) || typeof(TResult) == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
                : typeof(TResult) == typeof(string) ? stringValue
                : typeof(TResult) == typeof(byte) || typeof(TResult) == typeof(byte?) ? byte.Parse(stringValue)
                : typeof(TResult) == typeof(short) || typeof(TResult) == typeof(short?) ? short.Parse(stringValue)
                : typeof(TResult) == typeof(int) || typeof(TResult) == typeof(int?) ? int.Parse(stringValue)
                : typeof(TResult) == typeof(long) || typeof(TResult) == typeof(long?) ? long.Parse(stringValue)
                : typeof(TResult) == typeof(Half) || typeof(TResult) == typeof(Half?) ? Half.Parse(stringValue)
                : typeof(TResult) == typeof(float) || typeof(TResult) == typeof(float?) ? float.Parse(stringValue)
                : typeof(TResult) == typeof(double) || typeof(TResult) == typeof(double?) ? double.Parse(stringValue)
                : typeof(TResult) == typeof(decimal) || typeof(TResult) == typeof(decimal?) ? decimal.Parse(stringValue)
                : typeof(TResult) == typeof(DateTime) || typeof(TResult) == typeof(DateTime?) ? DateTime.Parse(stringValue)
                : typeof(TResult) == typeof(DateOnly) || typeof(TResult) == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
                : typeof(TResult) == typeof(TimeOnly) || typeof(TResult) == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
                : typeof(TResult) == typeof(Guid) || typeof(TResult) == typeof(Guid?) ? Guid.Parse(stringValue)
                : typeof(TResult) == typeof(IPAddress) ? IPAddress.Parse(stringValue)
                : typeof(TResult).GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, typeof(TResult))
                : throw new NotSupportedException();

            objOfType = (TResult) obj;
            return true;
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            objOfType = default;
            return false;
        }
    }

    public static object? ParseToObjectOfType(this object value, Type type)
    {
        string? stringValue = value.ToString();

        return stringValue is null ? Convert.ChangeType(value, type)
            : type == typeof(bool) || type == typeof(bool?) ? bool.Parse(stringValue)
            : type == typeof(char) || type == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
            : type == typeof(string) ? stringValue
            : type == typeof(byte) || type == typeof(byte?) ? byte.Parse(stringValue)
            : type == typeof(short) || type == typeof(short?) ? short.Parse(stringValue)
            : type == typeof(int) || type == typeof(int?) ? int.Parse(stringValue)
            : type == typeof(long) || type == typeof(long?) ? long.Parse(stringValue)
            : type == typeof(Half) || type == typeof(Half?) ? Half.Parse(stringValue)
            : type == typeof(float) || type == typeof(float?) ? float.Parse(stringValue)
            : type == typeof(double) || type == typeof(double?) ? double.Parse(stringValue)
            : type == typeof(decimal) || type == typeof(decimal?) ? decimal.Parse(stringValue)
            : type == typeof(DateTime) || type == typeof(DateTime?) ? DateTime.Parse(stringValue)
            : type == typeof(DateOnly) || type == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
            : type == typeof(TimeOnly) || type == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
            : type == typeof(Guid) || type == typeof(Guid?) ? Guid.Parse(stringValue)
            : type == typeof(IPAddress) ? IPAddress.Parse(stringValue)
            : type.GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, type)
            : throw new NotSupportedException();
    }

    public static object? ParseToObjectOfTypeOrDefault(this object value, Type type)
    {
        string? stringValue = value.ToString();

        try
        {
            return stringValue is null ? Convert.ChangeType(value, type)
                : type == typeof(bool) ? bool.Parse(stringValue)
                : type == typeof(char) || type == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
                : type == typeof(string) ? stringValue
                : type == typeof(byte) || type == typeof(byte?) ? byte.Parse(stringValue)
                : type == typeof(short) || type == typeof(short?) ? short.Parse(stringValue)
                : type == typeof(int) || type == typeof(int?) ? int.Parse(stringValue)
                : type == typeof(long) || type == typeof(long?) ? long.Parse(stringValue)
                : type == typeof(Half) || type == typeof(Half?) ? Half.Parse(stringValue)
                : type == typeof(float) || type == typeof(float?) ? float.Parse(stringValue)
                : type == typeof(double) || type == typeof(double?) ? double.Parse(stringValue)
                : type == typeof(decimal) || type == typeof(decimal?) ? decimal.Parse(stringValue)
                : type == typeof(DateTime) || type == typeof(DateTime?) ? DateTime.Parse(stringValue)
                : type == typeof(DateOnly) || type == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
                : type == typeof(TimeOnly) || type == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
                : type == typeof(Guid) || type == typeof(Guid?) ? Guid.Parse(stringValue)
                : type == typeof(IPAddress) ? IPAddress.Parse(stringValue)
                : type.GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, type)
                : throw new NotSupportedException();
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            return default;
        }
    }

    public static bool TryParseToObjectOfType(this object value, Type type, out object? objOfType)
    {
        string? stringValue = value.ToString();

        try
        {
            objOfType = stringValue is null ? Convert.ChangeType(value, type)
                : type == typeof(bool) ? bool.Parse(stringValue)
                : type == typeof(char) || type == typeof(char?) ? stringValue.Length is 1 ? stringValue.First() : throw new FormatException()
                : type == typeof(string) ? stringValue
                : type == typeof(byte) || type == typeof(byte?) ? byte.Parse(stringValue)
                : type == typeof(short) || type == typeof(short?) ? short.Parse(stringValue)
                : type == typeof(int) || type == typeof(int?) ? int.Parse(stringValue)
                : type == typeof(long) || type == typeof(long?) ? long.Parse(stringValue)
                : type == typeof(Half) || type == typeof(Half?) ? Half.Parse(stringValue)
                : type == typeof(float) || type == typeof(float?) ? float.Parse(stringValue)
                : type == typeof(double) || type == typeof(double?) ? double.Parse(stringValue)
                : type == typeof(decimal) || type == typeof(decimal?) ? decimal.Parse(stringValue)
                : type == typeof(DateTime) || type == typeof(DateTime?) ? DateTime.Parse(stringValue)
                : type == typeof(DateOnly) || type == typeof(DateOnly?) ? DateOnly.Parse(stringValue)
                : type == typeof(TimeOnly) || type == typeof(TimeOnly?) ? TimeOnly.Parse(stringValue)
                : type == typeof(Guid) || type == typeof(Guid?) ? Guid.Parse(stringValue)
                : type == typeof(IPAddress) ? IPAddress.Parse(stringValue)
                : type.GetInterfaces().Contains(typeof(IDatabaseEntity)) ? ParseFromJson(stringValue, type)
                : throw new NotSupportedException();

            return true;
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or JsonException)
        {
            objOfType = null;
            return false;
        }
    }

    private static object ParseFromJson(string json, Type targetType)
        => JsonSerializer.Deserialize(json, targetType, new JsonSerializerOptionsProvider().Default) ?? string.Empty;
}
