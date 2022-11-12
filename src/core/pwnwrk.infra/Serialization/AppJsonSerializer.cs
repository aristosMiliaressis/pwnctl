namespace pwnwrk.infra.Serialization;

using System.Text.Json;

public class AppJsonSerializer : ISerializer
{
    private static JsonSerializerOptions _options = JsonSerializerOptionsFactory.Create();

    public object Deserialize(string json, Type type)
    {
        return JsonSerializer.Deserialize(json, type, _options);
    }

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public T Deserialize<T>(JsonElement element)
    {
        return element.Deserialize<T>(_options);
    }

    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public string Serialize(object obj, Type type)
    {
        return JsonSerializer.Serialize(obj, type, _options);
    }
}