using System;
using System.Threading.Tasks;

using pwnctl.infra.Persistence;
using pwnctl.dto.Db.Commands;
using pwnctl.cli.Interfaces;
using pwnctl.app;

namespace pwnctl.cli.ModeHandlers
{
    public sealed class QueryModeHandler : ModeHandler
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

            var result = await PwnctlApiClient.Default.Send(command);

            if (result != null)
                Console.WriteLine(PwnInfraContext.Serializer.Serialize(result));
        }

        public void PrintHelpSection()
        {
            Console.WriteLine($"\t{ModeName}");
            Console.WriteLine("\t\tQuery mode (reads SQL from stdin executes and prints output to stdout)");
        }
    }
}