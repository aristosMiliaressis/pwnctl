namespace pwnctl.app.Common.Interfaces;

using System.Text.Json;

public interface Serializer
{
    T? Deserialize<T>(string json);
    T? Deserialize<T>(JsonElement element);
    object? Deserialize(string json, Type type);

    string Serialize<T>(T obj);
    string Serialize(object obj, Type type);
}