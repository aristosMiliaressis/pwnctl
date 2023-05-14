namespace pwnctl.infra.Serialization.JsonConverters;

using pwnctl.app.Common.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ShortNameJsonConverter : JsonConverter<ShortName>
{
    public override ShortName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ShortName.Create(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, ShortName shortNameValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(shortNameValue.Value);
    }
}
