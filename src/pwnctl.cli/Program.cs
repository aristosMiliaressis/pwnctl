using pwnctl.cli.ModeProviders;
using pwnctl.cli;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;


internal class Program
{
    static Dictionary<string, IModeProvider> _modeProviders = 
            AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => !t.IsInterface && typeof(IModeProvider).IsAssignableFrom(t))
                    .Select(t => (IModeProvider)Activator.CreateInstance(t)) 
                    .ToDictionary(p => p.ModeName, p => p);

    static async Task Main(string[] args)
    {
        await PwnctlAppFacade.SetupAsync();

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

        await provider.Run(args);
    }

    static void PrintHelpPage()
    {
        Console.WriteLine();
        Console.WriteLine($"USAGE: {Path.GetFileName(Assembly.GetExecutingAssembly().Location).Replace(".dll", "")} <mode> [arguments]");
        Console.WriteLine();
        Console.WriteLine("MODES:");
        foreach (var provider in _modeProviders.Values)
        {
            provider.PrintHelpSection();
        }
    }
}
