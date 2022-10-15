using System;
using System.Threading.Tasks;

using pwnctl.cli.Utilities;

namespace pwnctl.cli.ModeHandlers
{
    public class ProcessModeHandler : IModeHandler
    {
        public string ModeName => "process";
        
        public async Task Handle(string[] args)
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