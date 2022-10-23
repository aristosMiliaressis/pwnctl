using pwnwrk.cli.Utilities;
using pwnwrk.infra;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Contains("-h") || args.Contains("--help"))
        {
            //PrintHelpPage();
            return;
        }

        var processor = new AssetProcessor();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            await processor.TryProccessAsync(line);
        }
    }
}
