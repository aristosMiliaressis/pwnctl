namespace pwnwrk.infra.Serialization;

using System.Text.Json;

public static class JsonSerializerOptionsFactory
{
    public static JsonSerializerOptions Create()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
