namespace pwnctl.test.unit;

using pwnctl.cli.Repositories;
using pwnctl.cli.Utilities;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Logging;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.Entities;
using pwnwrk.domain.BaseClasses;
using pwnctl.cli;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using pwnwrk.infra.Configuration;

public class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_IsTestRun", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        
        PwnctlAppFacade.SetupAsync().Wait();

        var psi = new ProcessStartInfo();
        psi.FileName = "/bin/bash";
        psi.Arguments = " -c scripts/get_public_suffixes.sh";
        psi.EnvironmentVariables["PWNCTL_INSTALL_PATH"] = ".";
        psi.CreateNoWindow = true;

        using (var process = Process.Start(psi))
        {
            process?.WaitForExit();
        }
    }

    [Fact]
    public void AssetParser_Tests()
    {
        AssetParser.TryParse("example.com", out Type[] assetTypes, out BaseAsset[] assets);
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Keyword));
        Assert.Contains(assets, t => t.GetType() == typeof(Keyword));
        Assert.Equal(2, assets.Count());
        Assert.Equal(2, assetTypes.Count());

        AssetParser.TryParse("1.3.3.7", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Single(assets);
        Assert.Single(assetTypes);

        AssetParser.TryParse("76.24.104.208:65533", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Equal(2, assets.Length);
        Assert.Equal(2, assetTypes.Length);

        AssetParser.TryParse("76.24.104.208:U161", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Equal(2, assets.Length);
        Assert.Equal(2, assetTypes.Length);

        AssetParser.TryParse("172.16.17.0/24", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(NetRange));
        Assert.Contains(assets, t => t.GetType() == typeof(NetRange));
        Assert.Single(assets);
        Assert.Single(assetTypes);

        AssetParser.TryParse("xyz.example.com", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Equal(3, assets.Length);
        Assert.Equal(3, assetTypes.Length);

        AssetParser.TryParse("xyz.example.com IN A 31.3.3.7", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(DNSRecord));
        Assert.Contains(assets, t => t.GetType() == typeof(DNSRecord));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));

        AssetParser.TryParse("https://xyz.example.com:8443/api/token", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));

        AssetParser.TryParse("https://xyz.example.com:8443/api/token?_u=xxx", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint));


        AssetParser.TryParse("multi.level.sub.example.com", out assetTypes, out assets);
        Assert.Contains(assets, t => ((Domain)t).Name == "example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "sub.example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "level.sub.example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "multi.level.sub.example.com");

        AssetParser.TryParse("fqdn.example.com.", out assetTypes, out assets);
        Assert.Contains(assets, t => ((Domain)t).Name == "example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "fqdn.example.com");

        AssetParser.TryParse("no-reply@tesla.com", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        AssetParser.TryParse("no-reply@whatever.com", out assetTypes, out assets);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        bool res = AssetParser.TryParse("{\"asset\":\"https://whatever.tesla.com/.git\",\"tags\":{\"status\":403,\"location\":\"\",\"FoundBy\":\"dir_brute_common\"}}", out assetTypes, out assets);
        Assert.True(res);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        var tags = assets.First(a => a is Endpoint).Tags;
        Assert.Contains(tags, t => t.Name == "status" && t.Value == "403");
        Assert.Equal("dir_brute_common", assets.First(a => a is Endpoint).FoundBy);

        // TODO: SPF parsing test
        // TODO: test that tags overwrite model properties
    }

    [Fact]
    public async System.Threading.Tasks.Task ScopeChecker_Tests()
    {
        // net range
        Assert.True(ScopeChecker.Singleton.IsInScope(new NetRange("172.16.17.0", 24)));
        Assert.False(ScopeChecker.Singleton.IsInScope(new NetRange("172.16.16.0", 24)));

        // host in netrange
        Assert.True(ScopeChecker.Singleton.IsInScope(new Host("172.16.17.4")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Host("172.16.16.5")));

        // endpoint in net range
        Assert.True(ScopeChecker.Singleton.IsInScope(new Endpoint("https", new Service(new Host("172.16.17.15"), 443), "/api/token")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Endpoint("https", new Service(new Host("172.16.16.15"), 443), "/api/token")));

        // domain
        Assert.True(ScopeChecker.Singleton.IsInScope(new Domain("tesla.com")));
        Assert.True(ScopeChecker.Singleton.IsInScope(new Keyword(new Domain("tesla.com"), "tesla")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Keyword(new Domain("tttesla.com"), "tttesla")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tttesla.com")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla.com.net")));
        //Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla.com.test")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla2.com")));

        // Emails
        Assert.True(ScopeChecker.Singleton.IsInScope(new Email(new Domain("tesla.com"), "no-reply@tesla.com")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Email(new Domain("tesla2.com"), "no-reply@tesla2.com")));

        //subdomain
        Assert.True(ScopeChecker.Singleton.IsInScope(new Domain("xyz.tesla.com")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("xyz.tesla2.com")));

        // DNS records
        Assert.True(ScopeChecker.Singleton.IsInScope(new DNSRecord(DNSRecord.RecordType.A, "xyz.tesla.com", "1.3.3.7")));
        Assert.True(ScopeChecker.Singleton.IsInScope(new DNSRecord(DNSRecord.RecordType.A, "example.com", "172.16.17.15")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new DNSRecord(DNSRecord.RecordType.A, "example.com", "172.16.16.15")));

        // test for inscope host from domain relationship
        AssetProcessor processor = new();
        PwnctlDbContext context = new();
        await processor.ProcessAsync("xyz.tesla.com IN A 1.3.3.7");
        var host = context.Hosts.First(h => h.IP == "1.3.3.7");
        Assert.True(host.InScope);
    }

    [Fact]
    public async System.Threading.Tasks.Task JobAssignment_Tests()
    {
        JobAssignmentService jobService = new();
        PwnctlDbContext context = new();

        var netRange = new NetRange("172.16.17.0", 24);
        context.Add(netRange);
        context.SaveChanges();
        await jobService.AssignAsync(netRange);

        // blacklist test
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "nmap_basic"));

        var endpoint = new Endpoint("https", new Service(new Host("172.16.17.15"), 443), "/api/token");
        endpoint.AddTags(new List<Tag> {new Tag("Content-Type", "text/html")});
        context.Add(endpoint);
        context.SaveChanges();
        var service = context.Services.First(h => h.Origin == "tcp://172.16.17.15:443");
        await jobService.AssignAsync(endpoint);
        // TaskDefinition.Filter fail test
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "ffuf_common"));

        // aggresivness test
        Assert.True(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "hakrawler"));
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "sqlmap"));

        // Task.Command interpolation test
        var hakrawlerTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "hakrawler");
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15:443/api/token/", hakrawlerTask.Command);

        endpoint = new Endpoint("https", service, "/");
        context.Add(endpoint);
        context.SaveChanges();
        await jobService.AssignAsync(endpoint);
        
        // TaskDefinition.Filter pass test
        Assert.True(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "ffuf_common"));

        // multiple interpolation test
        var domain = new Domain("sub.tesla.com");
        context.Add(domain);
        context.SaveChanges();
        await jobService.AssignAsync(domain);
        var resolutionTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "domain_resolution");
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'| pwnctl process", resolutionTask.Command);

        var keyword = new Keyword(domain, "tesla");
        await jobService.AssignAsync(keyword);
        var cloudEnumTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        // TODO: AllowActive = false test, csv black&whitelist test
    }

    [Fact]
    public void PublicSuffixRepository_Tests()
    {
        var regDomain = PublicSuffixRepository.Singleton.GetRegistrationDomain("xyz.example.com");
        var publicSuffix = PublicSuffixRepository.Singleton.GetPublicSuffix("xyz.example.com");
        Assert.Equal("example.com", regDomain);
        Assert.Equal("com", publicSuffix.Suffix);

        regDomain = PublicSuffixRepository.Singleton.GetRegistrationDomain("sub.example.azurewebsites.net");
        publicSuffix = PublicSuffixRepository.Singleton.GetPublicSuffix("sub.example.azurewebsites.net");
        Assert.Equal("example.azurewebsites.net", regDomain);
        Assert.Equal("azurewebsites.net", publicSuffix.Suffix);
    }

    [Fact]
    public async System.Threading.Tasks.Task AssetRepository_Tests()
    {
        AssetRepository repository = new();
        PwnctlDbContext context = new();

        var inScopeDomain = new Domain("tesla.com");
        var outOfScope = new Domain("www.outofscope.com");

        Assert.False(repository.CheckIfExists(inScopeDomain));
        await repository.AddOrUpdateAsync(inScopeDomain);
        Assert.True(repository.CheckIfExists(inScopeDomain));
        inScopeDomain = context.Domains.First(d => d.Name == "tesla.com");
        await repository.AddOrUpdateAsync(outOfScope);
        outOfScope = context.Domains.First(d => d.Name == "www.outofscope.com");

        var record1 = new DNSRecord(DNSRecord.RecordType.A, "hackerone.com", "1.3.3.7");
        var record2 = new DNSRecord(DNSRecord.RecordType.AAAA, "hackerone.com", "dead:beef::::");

        Assert.False(repository.CheckIfExists(record1));
        Assert.False(repository.CheckIfExists(record2));
        await repository.AddOrUpdateAsync(record1);
        Assert.True(repository.CheckIfExists(record1));
        Assert.False(repository.CheckIfExists(record2));
        await repository.AddOrUpdateAsync(record2);
        Assert.True(repository.CheckIfExists(record2));

        var netRange = new NetRange("10.1.101.0", 24);
        Assert.False(repository.CheckIfExists(netRange));
        await repository.AddOrUpdateAsync(netRange);
        Assert.True(repository.CheckIfExists(netRange));

        var service = new Service(inScopeDomain, 443);
        Assert.False(repository.CheckIfExists(service));
        await repository.AddOrUpdateAsync(service);
        Assert.True(repository.CheckIfExists(service));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssetProcessor_Tests()
    {
        AssetProcessor processor = new();
        PwnctlDbContext context = new();

        var res = await processor.TryProccessAsync("tesla.com");
        Assert.True(res);

        var domain = context.Domains.First(d => d.Name == "tesla.com");
        Assert.True(domain.InScope);

        var keyword = context.Keywords.First(d => d.Word == "tesla");
        Assert.True(keyword.InScope);
        var cloudEnumTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        res = await processor.TryProccessAsync("tesla.com IN A 31.3.3.7");
        Assert.True(res);

        var record = context.DNSRecords.First(r => r.Key == "tesla.com" && r.Value == "31.3.3.7");
        Assert.True(record.InScope);

        var host = context.Hosts.Include(h => h.AARecords).First(host => host.IP == "31.3.3.7");
        Assert.True(host.InScope);
        host.AARecords.Add(record);
        Assert.NotNull(host.AARecords.First());
        Assert.True(host.AARecords.First().InScope);
        Assert.True(host.AARecords.First().Domain.InScope);
        var program = ScopeChecker.Singleton.GetApplicableProgram(host);
        Assert.NotNull(program);

        res = await processor.TryProccessAsync("85.25.105.204:65530");
        host = context.Hosts.First(h => h.IP == "85.25.105.204");
        Assert.True(res);
        var service = context.Services.First(srv => srv.Origin == "tcp://85.25.105.204:65530");
    }

    [Fact]
    public async System.Threading.Tasks.Task Tagging_Tests()
    {
        AssetProcessor processor = new();
        PwnctlDbContext context = new();

        var exampleUrl = new {
            asset = "https://example.com",
            tags = new Dictionary<string,string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        BaseAsset[] assets = AssetParser.Parse(JsonSerializer.Serialize(exampleUrl), out Type[] assetTypes);

        var endpoint = (Endpoint) assets.First(a => a.GetType() == typeof(Endpoint));
        Assert.NotNull(endpoint.Tags);
        var tags = assets.First(a => a is Endpoint).Tags;

        var ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        await processor.ProcessAsync(JsonSerializer.Serialize(exampleUrl));

        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://example.com:443/").First();
        ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        await processor.ProcessAsync(JsonSerializer.Serialize(exampleUrl));

        var teslaUrl = new
        {
            asset = "https://iis.tesla.com",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Protocol", "IIS"}
            }
        };

        // process same asset twice and make sure tasks are only assigned once
        await processor.ProcessAsync(JsonSerializer.Serialize(teslaUrl));
        endpoint = (Endpoint) context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://iis.tesla.com:443/").First();
        var tasks = context.Tasks.Include(t => t.Definition).Where(t => t.EndpointId == endpoint.Id).ToList();
        Assert.True(!tasks.GroupBy(t => t.DefinitionId).Any(g => g.Count() > 1));
        srvTag = endpoint.Tags.First(t => t.Name == "protocol");
        Assert.Equal("IIS", srvTag.Value);
        Assert.Contains(tasks, t => t.Definition.ShortName == "shortname_scanner");

        var apacheTeslaUrl = new
        {
            asset = "https://apache.tesla.com",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "apache"}
            }
        };

        // test Tag filter
        await processor.ProcessAsync(JsonSerializer.Serialize(apacheTeslaUrl));
        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://apache.tesla.com:443/").First();
        tasks = context.Tasks.Include(t => t.Definition).Where(t => t.EndpointId == endpoint.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.ShortName == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await processor.ProcessAsync(JsonSerializer.Serialize(sshService));
        var service = context.Services.Where(ep => ep.Origin == "tcp://1.3.3.7:22").First();
        Assert.Equal("ssh", service.ApplicationProtocol);
    }

    // [Fact]
    // public void QueryRunner_Tests()
    // {
    //     var queryRunner = new QueryRunner();

    //     try
    //     {
    //         queryRunner.Run("SELECT \"ShortName\" FROM \"Tasks\" JOIN \"TaskDefinitions\" ON \"Tasks\".\"DefinitionId\" = \"TaskDefinitions\".\"Id\"");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex.ToRecursiveExInfo());
    //     }
    // }

    //[Fact]
    // public void NotificationRuleChecker_Tests()
    // {
    //     // TODO: NotificationRuleChecker_Tests
    // }

    // [Fact]
    // public void IP_Host_Url_Normalization_Tests()
    // {
    // TODO: IP_Host_Url_Normalization_Tests
    // }
}