using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using pwnctl.app;
using pwnctl.cli.Interfaces;
using pwnctl.infra;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class ImportModeHandler : ModeHandler
    {
        public string ModeName => "import";

        public async Task Handle(string[] args)
        {
            if (args.Length < 2 || args[1] != "--path")
            {
                Console.WriteLine("--path option is required");
                PrintHelpSection();
                return;
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("No value provided for --path option");
                PrintHelpSection();
                return;
            }

            var path = args[2];

            Matcher matcher = new();
            matcher.AddInclude("*.json");

            PwnInfraContextInitializer.Setup(mock: true);
            await DatabaseInitializer.InitializeAsync();
            var context = new PwnctlDbContext();
            var processor = AssetProcessorFactory.Create();

            // var existingProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == command.Name);
            // if (existingProgram != null)
            //     continue;

            // context.Programs.Add(command);
            // await context.SaveChangesAsync();

            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                var assetLines = File.ReadAllLines(file);

                foreach (var asset in assetLines)
                {
                    await processor.TryProcessAsync(asset);
                }
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine($"\t\timports assets in jsonline format from the specified path.");
            Console.WriteLine($"\t\tArguments:");
            Console.WriteLine($"\t\t\t--path\tthe export path.");
        }
    }
}