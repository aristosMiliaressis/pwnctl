using System;
using System.Threading.Tasks;

using pwnwrk.infra.Persistence;
using pwnctl.dto.Db.Commands;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class QueryModeHandler : IModeHandler
    {
        public string ModeName => "query";
        
        public async Task Handle(string[] args)
        {
            var queryRunner = new QueryRunner();
            string line, query = string.Empty;

            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                query += line + "\n";
            }

            var command = new RunSqlQueryCommand
            {
                Query = query
            };

            var client = new PwnctlApiClient();
            await client.Send(command);        
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tQuery mode (reads SQL from stdin executes and prints output to stdout)");
        }
    }
}