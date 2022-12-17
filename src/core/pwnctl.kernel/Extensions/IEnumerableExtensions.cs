namespace pwnctl.kernel.Extensions;

public static class IEnumerableExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> func)
    {
        foreach (var value in list)
        {
            await func(value);
        }
    }
}
