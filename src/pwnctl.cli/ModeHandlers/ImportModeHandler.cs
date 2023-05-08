using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using pwnctl.cli.Interfaces;
using pwnctl.dto.Assets.Commands;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ImportModeHandler : ModeHandler
    {
        public string ModeName => "import";

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<ImportModeHandler>(args).WithParsedAsync(async opt =>
            {
                List<string> lines = new();
                string line;
                while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                {
                   lines.Add(line);
                }

                var request = new ImportAssetsCommand
                {
                    Assets = lines
                };

                await PwnctlApiClient.Default.Send(request);
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\timports assets in raw format from stdin.");
        }
    }
}
