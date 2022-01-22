using CommandLine;
using Microsoft.Extensions.Configuration;
using pwnctl;
using pwnctl.DataEF;
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
        private static readonly PwntainerDbContext _dbContext = new();

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    if (opts.QueryMode)
                    {
                        await QueryModeAsync();
                        return;
                    }
                    
                    await ProcessModeAsync();
                },
                errs => Task.FromResult(-1));
        }

        private static async Task QueryModeAsync()
        {
            var input = new List<string>();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                input.Add(line);
            }

            await _dbContext.RunSQLAsync(string.Join("\n", input));
        }

        private static async Task ProcessModeAsync()
        {
            var assetService = new AssetService();

            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                await assetService.ProcessAsync(line);
            }
        }
    }
}
