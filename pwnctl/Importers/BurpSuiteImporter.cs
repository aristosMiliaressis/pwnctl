using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace pwnctl.Importers
{
    public static class BurpSuiteImporter
    {
        public static async Task ImportAsync(string file)
        {
            var csv = File.ReadLines(file).Skip(1);
            foreach (var line in csv)
            {
                var parts = line.Split(",");
            }

            throw new NotImplementedException();
        }
    }
}