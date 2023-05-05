using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
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

        [Option('p', "path", Required = true, HelpText = "Path to the import file.")]
        public string ImportPath { get; set; }

        public async Task Handle(string[] args)
        {
           await Parser.Default.ParseArguments<ImportModeHandler>(args).WithParsedAsync(async opt =>
           {
               Matcher matcher = new();
               matcher.AddInclude("*.json");

               //PwnInfraContextInitializer.Setup();
            //    await DatabaseInitializer.InitializeAsync();
            //    var context = new PwnctlDbContext();
                var processor = AssetProcessorFactory.Create();

               // var existingProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == command.Name);
               // if (existingProgram != null)
               //     continue;

               // context.Programs.Add(command);
               // await context.SaveChangesAsync();

               foreach (string file in matcher.GetResultsInFullPath(opt.ImportPath))
               {
                   var assetLines = File.ReadAllLines(file);

                   foreach (var asset in assetLines)
                   {
                       await processor.TryProcessAsync(asset, null);
                   }
               }
           });
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}\timports assets in jsonline format from the specified path.");
            Console.WriteLine($"\t\t--path\tthe export path.");
        }
    }
}
