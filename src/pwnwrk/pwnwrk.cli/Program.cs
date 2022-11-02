using pwnwrk.infra.Utilities;
using System;
using System.Threading.Tasks;

internal sealed class Program
{
    static async Task Main(string[] args)
    {
        var processor = new AssetProcessor();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            await processor.TryProccessAsync(line);
        }
    }
}
