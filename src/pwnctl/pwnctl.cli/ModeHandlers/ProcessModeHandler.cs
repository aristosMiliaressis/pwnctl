using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnctl.dto.Process.Commands;

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
            await client.Send(command);
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tAsset processing mode (reads assets from stdin)");
        }
    }
}