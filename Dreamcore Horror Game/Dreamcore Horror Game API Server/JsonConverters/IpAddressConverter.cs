using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer.JsonConverters;

public class IpAddressConverter : JsonConverter<IPAddress>
{
    public override bool HandleNull => true;

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert == typeof(IPAddress);

    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());

    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => IPAddress.Parse(reader.GetString()!);
}
