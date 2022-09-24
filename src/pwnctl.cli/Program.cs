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

await PwnctlAppFacade.SetupAsync();
var app = CoconaApp.Create();

app.AddCommand("query", async () =>
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
        assets.ToList().ForEach(a => Console.WriteLine(a.ToJson() + "\n"));
    }
    AssetRepository repository = new();
    if (mode.ToLower() == "hosts")
    {
        var assets = repository.ListHosts().Select(a => (BaseAsset)a);
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
    if (mode.ToLower() == "emails")
    {
        var assets = repository.ListEmails().Select(a => (BaseAsset)a);
        WriteToConsole(assets);
    }
}).WithDescription("List assets");

app.AddCommand("export", (string path) =>
{
    void WriteToFile(string filename, IEnumerable<BaseAsset> assets)
    {
        assets.ToList().ForEach(a => File.AppendAllText(filename, a.ToJson() + "\n"));
    }

    try
    {
        Directory.CreateDirectory(path);
    }
    catch (Exception ex)
    {
        throw new Exception($"Unable to create export directory {path}", ex);
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
    var emails = repository.ListEmails().Select(a => (BaseAsset)a);
    WriteToFile(Path.Combine(path, "emails.json"), emails);
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
    int emailCount = context.Emails.Count();
    int tagCount = context.Tags.Count();
    int inScopeRangesCount = context.NetRanges.Where(a => a.InScope).Count();
    int insCopeHostCount = context.Hosts.Count();
    int inScopeDomainCount = context.Domains.Where(a => a.InScope).Count();
    int inScopeRecordCount = context.DNSRecords.Where(a => a.InScope).Count();
    int inScopeServiceCount = context.Services.Where(a => a.InScope).Count();
    int inScopeEndpointCount = context.Endpoints.Where(a => a.InScope).Count();
    int inScopeParamCount = context.Parameters.Where(a => a.InScope).Count();
    int inScopeEmailCount = context.Emails.Where(a => a.InScope).Count();
    var firstTask = context.Tasks.OrderBy(t => t.QueuedAt).FirstOrDefault();
    var lastTask = context.Tasks.OrderBy(t => t.QueuedAt).FirstOrDefault();

    Console.WriteLine($"NetRanges: {netRangeCount}, InScope: {inScopeRangesCount}");
    Console.WriteLine($"Hosts: {hostCount}, InScope: {insCopeHostCount}");
    Console.WriteLine($"Domains: {domainCount}, InScope: {inScopeDomainCount}");
    Console.WriteLine($"DNSRecords: {recordCount}, InScope: {inScopeRecordCount}");
    Console.WriteLine($"Services: {serviceCount}, InScope: {inScopeServiceCount}");
    Console.WriteLine($"Endpoints: {endpointCount}, InScope: {inScopeEndpointCount}");
    Console.WriteLine($"Parameters: {paramCount}, InScope: {inScopeParamCount}");
    Console.WriteLine($"Emais: {emailCount}, InScope: {inScopeEmailCount}");
    Console.WriteLine($"Tags: {tagCount}");
    if (firstTask != null)
    {
        Console.WriteLine();
        Console.WriteLine("First Queued Task: " + firstTask.QueuedAt);
        Console.WriteLine("Last Queued Task: " + lastTask.QueuedAt);
    }    
});

app.Run();
