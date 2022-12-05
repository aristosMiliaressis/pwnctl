using pwnwrk.infra.Utilities;
using pwnwrk.infra.Queues;
using System;
using System.Threading.Tasks;

internal sealed class Program
{
    static async Task Main(string[] args)
    {
        var queueService = JobQueueFactory.Create();
        var processor = new AssetProcessor(queueService);

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            await processor.TryProccessAsync(line);
        }
    }
}
