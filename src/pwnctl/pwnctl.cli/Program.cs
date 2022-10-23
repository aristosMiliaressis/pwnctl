using pwnctl.cli.ModeHandlers;
using pwnwrk.infra;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class Program
{
    static Dictionary<string, IModeHandler> _modeProviders = 
            AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => !t.IsInterface && typeof(IModeHandler).IsAssignableFrom(t))
                    .Select(t => (IModeHandler)Activator.CreateInstance(t)) 
                    .ToDictionary(p => p.ModeName, p => p);

    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("No mode provided");
            PrintHelpPage();
            return;
        }

        var mode = args[0];

        if (!_modeProviders.ContainsKey(mode))
        {
            Console.WriteLine("Invalid Mode " + mode);
            PrintHelpPage();
            return;
        }

        var provider = _modeProviders[mode];

        await provider.Handle(args);
    }

    static void PrintHelpPage()
    {
        Console.WriteLine();
        Console.WriteLine($"USAGE: {Assembly.GetExecutingAssembly().GetName().Name} <mode> [arguments]");
        Console.WriteLine();
        Console.WriteLine("MODES:");
        foreach (var provider in _modeProviders.Values)
        {
            provider.PrintHelpSection();
        }
    }
}
