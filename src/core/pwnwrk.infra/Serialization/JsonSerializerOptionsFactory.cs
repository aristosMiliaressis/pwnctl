namespace pwnwrk.infra.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonSerializerOptionsFactory
{
    public static JsonSerializerOptions Create()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }
}
