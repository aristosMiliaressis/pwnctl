namespace pwnctl.infra.Serialization.JsonConverters;

using pwnctl.app.Common.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CronExpressionJsonConverter : JsonConverter<CronExpression>
{
    public override CronExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return CronExpression.Create(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, CronExpression cronExpressionValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(cronExpressionValue.Value);
    }
}
