namespace pwnctl.infra.Serialization.JsonConverters;

using pwnctl.domain.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

public class AssetClassJsonConverter : JsonConverter<AssetClass>
{
    public override AssetClass Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return AssetClass.Create(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, AssetClass assetClassValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(assetClassValue.Value);
    }
}
