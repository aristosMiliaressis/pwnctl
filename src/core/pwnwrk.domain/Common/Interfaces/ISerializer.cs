namespace pwnwrk.domain.Common.Interfaces;

using System.Text.Json;

public interface ISerializer : IAmbientService
{
    T Deserialize<T>(string json);
    T Deserialize<T>(JsonElement element);
    object Deserialize(string json, Type type);

    string Serialize<T>(T obj);
    string Serialize<T>(T obj, Type type);
}