using CommandLine;
using Microsoft.Extensions.Configuration;
using pwnctl;
using pwnctl.Persistence;
using pwnctl.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace pwnctl
{
    class Program
    {
        private static readonly QueryRunner _queryRunner = new QueryRunner(PwntainerDbContext.ConnectionString);
        private static readonly AssetService _assetService = new();

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    if (opts.QueryMode)
                    {
                        QueryMode();
                        return;
                    }
                    
                    await ProcessModeAsync();
                },
                errs => Task.FromResult(-1));
        }

        private static void QueryMode()
        {
            var input = new List<string>();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                input.Add(line);
            }

            _queryRunner.Run(string.Join("\n", input));
        }

        private static async Task ProcessModeAsync()
        {
            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                await _assetService.ProcessAsync(line);
            }
        }
    }
}
