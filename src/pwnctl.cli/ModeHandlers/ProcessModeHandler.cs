using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Commands;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using pwnctl.cli.Interfaces;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ProcessModeHandler : ModeHandler
    {
        public string ModeName => "process";

        public async Task Handle(string[] args)
        {
            var input = new List<string>();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                input.Add(line);
            }

            var command = new ProcessAssetsCommand
            {
                Assets = input
            };

            var client = new PwnctlApiClient();
            var queueService = new SQSTaskQueueService();

            var pendingTasks = await client.Send(command);
            if (pendingTasks == null)
                return;

            foreach (var task in pendingTasks)
            {
                try
                {
                    await queueService.EnqueueAsync(task);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToRecursiveExInfo());
                }
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tAsset processing mode (reads assets from stdin)");
        }
    }
}