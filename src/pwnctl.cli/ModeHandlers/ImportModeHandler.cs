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

        [Option('b', "batch", Required = false, HelpText = "Batch size.")]
        public int BatchSize { get; set; }

        public async Task Handle(string[] args)
        {
            await Parser.Default.ParseArguments<ImportModeHandler>(args).WithParsedAsync(async opt =>
            {
                if (opt.BatchSize == 0)
                    opt.BatchSize = 50;

                List<string> lines = new();
                string line;
                while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                {
                    lines.Add(line);
                    if (lines.Count == opt.BatchSize)
                    {
                        var request = new ImportAssetsCommand
                        {
                            Assets = lines
                        };

                        await Program.Sender.Send(request);

                        lines = new();
                    }
                }

                if (lines.Count != 0)
                {
                    var request = new ImportAssetsCommand
                    {
                        Assets = lines
                    };

                    await Program.Sender.Send(request);
                }
            });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\timports assets in raw format from stdin.");
        }
    }
}
