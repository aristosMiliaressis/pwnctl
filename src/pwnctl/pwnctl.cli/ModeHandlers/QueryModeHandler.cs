using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using pwnwrk.infra.Persistence;

namespace pwnctl.cli.ModeHandlers
{
    public class QueryModeHandler : IModeHandler
    {
        public string ModeName => "query";
        
        public async Task Handle(string[] args)
        {
            var queryRunner = new QueryRunner();
            var input = new List<string>();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                input.Add(line);
            }

            await queryRunner.RunAsync(string.Join("\n", input));
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tQuery mode (reads SQL from stdin executes and prints output to stdout)");
        }
    }
}