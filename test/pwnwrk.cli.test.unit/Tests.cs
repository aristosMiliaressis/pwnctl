namespace pwnwrk.test.unit;

using pwnwrk.infra;
using pwnwrk.infra.Utilities;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.infra.Repositories;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Assets.BaseClasses;
using Microsoft.EntityFrameworkCore;
using pwnwrk.domain.Assets.Enums;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_IsTestRun", "true");
        Environment.SetEnvironmentVariable("PWNCTL_InstallPath", ".");

        // reset the database for every test method
        DatabaseInitializer.InitializeAsync().Wait();
    }

    [Fact]
    public void AssetParser_Tests()
    {
        AssetParser.TryParse("example.com", out Type[] assetTypes, out Asset[] assets);
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

        // subdirectory parsing test
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint) && ((Endpoint)t).Url == "https://xyz.example.com:8443/api/");

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
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        var tags = assets.First(a => a is Endpoint).Tags;
        Assert.Contains(tags, t => t.Name == "status" && t.Value == "403");
        Assert.Equal("dir_brute_common", assets.First(a => a is Endpoint).FoundBy);
        Assert.True(assets.All(a => a.FoundBy == "dir_brute_common"));

        // TODO: SPF parsing test
        // TODO: test that tags overwrite model properties
    }

    [Fact]
    public async System.Threading.Tasks.Task ScopeChecking_Tests()
    {
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        // net range
        Assert.NotNull(new NetRange(System.Net.IPAddress.Parse("172.16.17.0"), 24).GetOwningProgram(programs));
        Assert.Null(new NetRange(System.Net.IPAddress.Parse("172.16.16.0"), 24).GetOwningProgram(programs));

        // host in netrange
        Assert.NotNull(new Host("172.16.17.4").GetOwningProgram(programs));
        Assert.Null(new Host("172.16.16.5").GetOwningProgram(programs));

        // endpoint in net range
        Assert.NotNull(new Endpoint("https", new Service(new Host("172.16.17.15"), 443), "/api/token").GetOwningProgram(programs));
        Assert.Null(new Endpoint("https", new Service(new Host("172.16.16.15"), 443), "/api/token").GetOwningProgram(programs));

        // domain
        Assert.NotNull(new Domain("tesla.com").GetOwningProgram(programs));
        Assert.NotNull(new Keyword(new Domain("tesla.com"), "tesla").GetOwningProgram(programs));
        Assert.Null(new Keyword(new Domain("tttesla.com"), "tttesla").GetOwningProgram(programs));
        Assert.Null(new Domain("tttesla.com").GetOwningProgram(programs));
        Assert.Null(new Domain("tesla.com.net").GetOwningProgram(programs));
        //Assert.Null(new Domain("tesla.com.test").GetOwningProgram(programs));
        Assert.Null(new Domain("tesla2.com").GetOwningProgram(programs));

        // Emails
        Assert.NotNull(new Email(new Domain("tesla.com"), "no-reply@tesla.com").GetOwningProgram(programs));
        Assert.Null(new Email(new Domain("tesla2.com"), "no-reply@tesla2.com").GetOwningProgram(programs));

        //subdomain
        Assert.NotNull(new Domain("xyz.tesla.com").GetOwningProgram(programs));
        Assert.Null(new Domain("xyz.tesla2.com").GetOwningProgram(programs));

        // DNS records
        Assert.NotNull(new DNSRecord(DnsRecordType.A, "xyz.tesla.com", "1.3.3.7").GetOwningProgram(programs));
        Assert.NotNull(new DNSRecord(DnsRecordType.A, "example.com", "172.16.17.15").GetOwningProgram(programs));
        Assert.Null(new DNSRecord(DnsRecordType.A, "example.com", "172.16.16.15").GetOwningProgram(programs));

        // test for inscope host from domain relationship
        AssetProcessor processor = new(new MockJobQueueService());
        await processor.ProcessAsync("xyz.tesla.com IN A 1.3.3.7");
        var host = context.Hosts.First(h => h.IP == "1.3.3.7");
        Assert.True(host.InScope);
    }

    [Fact]
    public async System.Threading.Tasks.Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        AssetProcessor processor = new(new MockJobQueueService());

        // blacklist test
        await processor.ProcessAsync("172.16.17.0/24");
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "nmap_basic"));
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "ffuf_common"));

        var exampleUrl = new
        {
            asset = "https://172.16.17.15/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // TaskDefinition.Filter fail test
        await processor.ProcessAsync(PwnContext.Serializer.Serialize(exampleUrl));

        // aggresivness test
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "hakrawler"));
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "sqlmap"));

        // Task.Command interpolation test
        var hakrawlerTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "hakrawler");
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15:443/api/token/", hakrawlerTask.Command);

        // TaskDefinition.Filter pass test
        await processor.ProcessAsync("https://172.16.17.15/");
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "ffuf_common"));

        // multiple interpolation test
        await processor.ProcessAsync("sub.tesla.com");
        var resolutionTask = context.JoinedTaskRecordQueryable()
                                    .First(t => t.Domain.Name == "sub.tesla.com" 
                                             && t.Definition.ShortName == "domain_resolution");
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'| pwnctl process", resolutionTask.Command);

        // Keyword test
        var cloudEnumTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        // TODO: AllowActive = false test, csv black&whitelist test
    }

    [Fact]
    public void PublicSuffixRepository_Tests()
    {
        var exampleDomain = new Domain("xyz.example.com");

        Assert.Equal("example.com", exampleDomain.GetRegistrationDomain());
        Assert.Equal("com", exampleDomain.GetPublicSuffix().Suffix);

        var exampleSubDomain = new Domain("sub.example.azurewebsites.net");

        Assert.Equal("example.azurewebsites.net", exampleSubDomain.GetRegistrationDomain());
        Assert.Equal("azurewebsites.net", exampleSubDomain.GetPublicSuffix().Suffix);
    }

    [Fact]
    public async System.Threading.Tasks.Task AssetRepository_Tests()
    {
        AssetDbRepository repository = new();
        PwnctlDbContext context = new();

        var inScopeDomain = new Domain("tesla.com");
        var outOfScope = new Domain("www.outofscope.com");

        Assert.Null(context.FindAsset(inScopeDomain));
        await repository.SaveAsync(inScopeDomain);
        Assert.NotNull(context.FindAsset(inScopeDomain));
        inScopeDomain = context.Domains.First(d => d.Name == "tesla.com");
        await repository.SaveAsync(outOfScope);
        outOfScope = context.Domains.First(d => d.Name == "www.outofscope.com");

        var record1 = new DNSRecord(DnsRecordType.A, "hackerone.com", "1.3.3.7");
        var record2 = new DNSRecord(DnsRecordType.AAAA, "hackerone.com", "dead:beef::::");

        Assert.Null(context.FindAsset(record1));
        Assert.Null(context.FindAsset(record2));
        await repository.SaveAsync(record1);
        Assert.NotNull(context.FindAsset(record1));
        Assert.Null(context.FindAsset(record2));
        await repository.SaveAsync(record2);
        Assert.NotNull(context.FindAsset(record2));

        var netRange = new NetRange(System.Net.IPAddress.Parse("10.1.101.0"), 24);
        Assert.Null(context.FindAsset(netRange));
        await repository.SaveAsync(netRange);
        Assert.NotNull(context.FindAsset(netRange));

        var service = new Service(inScopeDomain, 443);
        Assert.Null(context.FindAsset(service));
        await repository.SaveAsync(service);
        Assert.NotNull(context.FindAsset(service));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssetProcessor_Tests()
    {
        AssetProcessor processor = new(new MockJobQueueService());
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        var res = await processor.TryProccessAsync("tesla.com");
        Assert.True(res);

        var domain = context.Domains.First(d => d.Name == "tesla.com");
        Assert.True(domain.InScope);

        var keyword = context.Keywords.First(d => d.Word == "tesla");
        Assert.True(keyword.InScope);
        var cloudEnumTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "cloud_enum");
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
        Assert.NotNull(host.GetOwningProgram(programs));

        res = await processor.TryProccessAsync("85.25.105.204:65530");
        host = context.Hosts.First(h => h.IP == "85.25.105.204");
        Assert.True(res);
        var service = context.Services.First(srv => srv.Origin == "tcp://85.25.105.204:65530");
    }

    [Fact]
    public async System.Threading.Tasks.Task Tagging_Tests()
    {
        AssetProcessor processor = new(new MockJobQueueService());
        PwnctlDbContext context = new();

        var exampleUrl = new {
            asset = "https://example.com",
            tags = new Dictionary<string,string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        Asset[] assets = AssetParser.Parse(PwnContext.Serializer.Serialize(exampleUrl), out Type[] assetTypes);

        var endpoint = (Endpoint) assets.First(a => a.GetType() == typeof(Endpoint));
        Assert.NotNull(endpoint.Tags);
        var tags = assets.First(a => a is Endpoint).Tags;

        var ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        await processor.ProcessAsync(PwnContext.Serializer.Serialize(exampleUrl));

        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://example.com:443/").First();
        ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        await processor.ProcessAsync(PwnContext.Serializer.Serialize(exampleUrl));

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
        await processor.ProcessAsync(PwnContext.Serializer.Serialize(teslaUrl));
        endpoint = (Endpoint) context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://iis.tesla.com:443/").First();
        var tasks = context.JoinedTaskRecordQueryable().Where(t => t.EndpointId == endpoint.Id).ToList();
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
        await processor.ProcessAsync(PwnContext.Serializer.Serialize(apacheTeslaUrl));
        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Url == "https://apache.tesla.com:443/").First();
        tasks = context.JoinedTaskRecordQueryable().Where(t => t.EndpointId == endpoint.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.ShortName == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await processor.ProcessAsync(PwnContext.Serializer.Serialize(sshService));
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