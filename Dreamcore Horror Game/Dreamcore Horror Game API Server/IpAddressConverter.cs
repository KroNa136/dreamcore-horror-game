using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer;

public class IpAddressConverter : JsonConverter<IPAddress>
{
    public override bool HandleNull => true;

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(IPAddress);
    }

    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        base.WriteAsPropertyName(writer, value, options);
    }

    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return IPAddress.Parse(reader.GetString()!);
    }

    public override IPAddress ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return base.ReadAsPropertyName(ref reader, typeToConvert, options);
    }
}