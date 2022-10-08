using System;
using System.Threading.Tasks;

using pwnctl.app.Utilities;

namespace pwnctl.cli.ModeProviders
{
    public class ProcessModeProvider : IModeProvider
    {
        public string ModeName => "process";
        
        public async Task Run(string[] args)
        {
            var processor = new AssetProcessor();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                await processor.TryProccessAsync(line);
            }
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tAsset processing mode (reads assets from stdin)");
        }
    }
}