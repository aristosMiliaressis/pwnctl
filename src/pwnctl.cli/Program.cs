using Cocona;
using pwnctl.infra.Persistence;
using pwnctl.infra.Logging;
using pwnctl.app.Importers;
using pwnctl.app.Utilities;
using pwnctl.app.Repositories;
using pwnctl.app;
using pwnctl.core.BaseClasses;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

PwnctlAppFacade.Setup();
var app = CoconaApp.Create();

app.AddCommand("query", () =>
    {
        var queryRunner = new QueryRunner(PwnctlDbContext.ConnectionString);
        var input = new List<string>();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            input.Add(line);
        }

        queryRunner.Run(string.Join("\n", input));
    }
).WithDescription("Query mode (reads SQL from stdin executes and prints output to stdout)");

app.AddCommand("process", async () => 
    {
        var processor = new AssetProcessor();

        string line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            try
            {
                await processor.TryProccessAsync(line);
            }
            catch (Exception ex)
            {
                Logger.Instance.Info(line);
                Logger.Instance.Info(ex.ToRecursiveExInfo());
            }
        }
    }
).WithDescription("Asset processing mode (reads assets from stdin)");

app.AddCommand("list", (string mode) => 
{
    void WriteToConsole(IEnumerable<BaseAsset> assets)
    {
        assets.ToList().ForEach(a => Console.WriteLine(a.ToJson()+ "\n"));
    }
    AssetRepository repository = new();
    if (mode.ToLower() == "hosts") 
    {
        var assets = repository.ListHosts().Select(a => (BaseAsset) a);
        WriteToConsole(assets);
    }
    if (mode.ToLower() == "endpoints")
    {
        var assets = repository.ListEndpoints().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
    if (mode.ToLower() == "domains")
    {
        var assets = repository.ListDomains().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
    if (mode.ToLower() == "services")
    {
        var assets = repository.ListServices().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
    if (mode.ToLower() == "dnsrecords")
    {
        var assets = repository.ListDNSRecords().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
    if (mode.ToLower() == "netranges")
    {
        var assets = repository.ListNetRanges().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
}
).WithDescription("List assets");

app.AddCommand("export", (string path) => {
    void WriteToFile(string filename, IEnumerable<BaseAsset> assets)
    {
        assets.ToList().ForEach(a => File.AppendAllText(filename, a.ToJson()+"\n"));
    }

    AssetRepository repository = new();
    var hosts = repository.ListHosts().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "hosts.json"), hosts);
    var endpoints = repository.ListEndpoints().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "endpoints.json"), endpoints);
    var domains = repository.ListDomains().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "domains.json"), domains);
    var services = repository.ListServices().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "services.json"), services);
    var records = repository.ListDNSRecords().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "records.json"), records);
    var netRanges = repository.ListNetRanges().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "netRanges.json"), netRanges);
});

app.AddCommand("summary", () =>
{
    PwnctlDbContext context = new();
    int netRangeCount = context.NetRanges.Count();
    int hostCount = context.Hosts.Count();
    int domainCount = context.Domains.Count();
    int recordCount = context.DNSRecords.Count();
    int serviceCount = context.Services.Count();
    int endpointCount = context.Endpoints.Count();
    int paramCount = context.Parameters.Count();
    int tagCount = context.Tags.Count();

    Console.WriteLine("NetRanges: " + netRangeCount);
    Console.WriteLine("Hosts: " + hostCount);
    Console.WriteLine("Domains: " + domainCount);
    Console.WriteLine("DNSRecords: " + recordCount);
    Console.WriteLine("Services: " + serviceCount);
    Console.WriteLine("Endpoints: " + endpointCount);
    Console.WriteLine("Parameters: " + paramCount);
    Console.WriteLine("Tags: " + tagCount);
});

app.Run();
