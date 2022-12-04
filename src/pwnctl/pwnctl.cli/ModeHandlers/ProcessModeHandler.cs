using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.dto.Assets.Commands;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Logging;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ProcessModeHandler : IModeHandler
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
            var queueService = JobQueueFactory.Create();

            var pendingTasks = await client.Send(command);
            if (pendingTasks == null)
                return;

            foreach (var task in pendingTasks)
            {
                try
                {
                    await queueService.EnqueueAsync(task);
                    Console.WriteLine($"Queued: {task.Command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToRecursiveExInfo());
                    Console.WriteLine(task.Definition.CommandTemplate);
                    Console.WriteLine(task.Discriminator);
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