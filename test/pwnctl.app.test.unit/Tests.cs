namespace pwnctl.app.test.unit;

using pwnctl.infra;
using pwnctl.infra.Queues;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Repositories;
using pwnctl.domain.Entities;
using pwnctl.domain.BaseClasses;
using Microsoft.EntityFrameworkCore;
using pwnctl.domain.Enums;
using System.Net;
using pwnctl.app.Assets;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Assets.DTO;

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
        Asset[] assets = AssetParser.Parse("example.com", out Type[] assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Keyword));
        Assert.Contains(assets, t => t.GetType() == typeof(Keyword));
        Assert.Equal(2, assets.Count());
        Assert.Equal(2, assetTypes.Count());

        assets = AssetParser.Parse("1.3.3.7", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Single(assets);
        Assert.Single(assetTypes);

        assets = AssetParser.Parse("76.24.104.208:65533", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Equal(2, assets.Length);
        Assert.Equal(2, assetTypes.Length);

        assets = AssetParser.Parse("76.24.104.208:U161", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Equal(2, assets.Length);
        Assert.Equal(2, assetTypes.Length);

        assets = AssetParser.Parse("172.16.17.0/24", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(NetRange));
        Assert.Contains(assets, t => t.GetType() == typeof(NetRange));
        Assert.Single(assets);
        Assert.Single(assetTypes);

        assets = AssetParser.Parse("xyz.example.com", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Equal(3, assets.Length);
        Assert.Equal(3, assetTypes.Length);

        assets = AssetParser.Parse("xyz.example.com IN A 31.3.3.7", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(DNSRecord));
        Assert.Contains(assets, t => t.GetType() == typeof(DNSRecord));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.Contains(assets, t => t.GetType() == typeof(Host));

        assets = AssetParser.Parse("https://xyz.example.com:8443/api/token", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));

        // subdirectory parsing test
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint) && ((Endpoint)t).Url == "https://xyz.example.com:8443/api/");

        assets = AssetParser.Parse("https://xyz.example.com:8443/api/token?_u=xxx", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assets, t => t.GetType() == typeof(Endpoint));

        assets = AssetParser.Parse("multi.level.sub.example.com", out assetTypes);
        Assert.Contains(assets, t => ((Domain)t).Name == "example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "sub.example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "level.sub.example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "multi.level.sub.example.com");

        assets = AssetParser.Parse("fqdn.example.com.", out assetTypes);
        Assert.Contains(assets, t => ((Domain)t).Name == "example.com");
        Assert.Contains(assets, t => ((Domain)t).Name == "fqdn.example.com");

        assets = AssetParser.Parse("no-reply@tesla.com", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        assets = AssetParser.Parse("mailto:test@tesla.com", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        assets = AssetParser.Parse("maito:test@tesla.com", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        assets = AssetParser.Parse("no-reply@whatever.com", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Email));
        Assert.Contains(assets, t => t.GetType() == typeof(Email));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));

        assets = AssetParser.Parse("{\"asset\":\"https://whatever.tesla.com/.git\",\"tags\":{\"status\":403,\"location\":\"\",\"FoundBy\":\"dir_brute_common\"}}", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assets, t => t.GetType() == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assets, t => t.GetType() == typeof(Domain));
        var asset = assets.First(a => a is Endpoint);
        Assert.Contains(asset.Tags, t => t.Name == "status" && t.Value == "403");
        Assert.True(assets.All(a => a.FoundBy == "dir_brute_common"));

        var spfRecord = "tesla.com IN TXT \"v = spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net - all\"";
        assets = AssetParser.Parse(spfRecord, out assetTypes);
        Assert.Equal(4, assets.Count());
        Assert.Contains(assetTypes, t => t == typeof(Domain));
        Assert.Contains(assetTypes, t => t == typeof(DNSRecord));
        Assert.Contains(assetTypes, t => t == typeof(Host));

        assets = AssetParser.Parse("FD00:DEAD:BEEF:64:35::2", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Host));
        Assert.NotEmpty(assets);

        assets = AssetParser.Parse("2001:db8::/48", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(NetRange));

        assets = AssetParser.Parse("[FD00:DEAD:BEEF:64:35::2]:163", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Host));

        assets = AssetParser.Parse("http://[FD00:DEAD:BEEF:64:35::2]:80/ipv6test", out assetTypes);
        Assert.Contains(assetTypes, t => t == typeof(Endpoint));
        Assert.Contains(assetTypes, t => t == typeof(Service));
        Assert.Contains(assetTypes, t => t == typeof(Host));

        //NetRagne.RouteTo(ipv4|ipv6)
        //Parameters/VirtualHosts/CloudServices
        // SPF ipv6 parsing, spfv1 vs spfv2?
        // PTR records
    }

    [Fact]
    public async Task ScopeChecking_Tests()
    {
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        // net range
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetRange(System.Net.IPAddress.Parse("172.16.17.0"), 24)))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetRange(System.Net.IPAddress.Parse("172.16.16.0"), 24)))));

        // host in netrange
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Host(IPAddress.Parse("172.16.17.4"))))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Host(IPAddress.Parse("172.16.16.5"))))));

        // endpoint in net range
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Endpoint("https", new Service(new Host(IPAddress.Parse("172.16.17.15")), 443), "/api/token")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Endpoint("https", new Service(new Host(IPAddress.Parse("172.16.16.15")), 443), "/api/token")))));

        // domain
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("tesla.com")))));
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Keyword(new Domain("tesla.com"), "tesla")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Keyword(new Domain("tttesla.com"), "tttesla")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("tttesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("tesla.com.net")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("tesla2.com")))));

        // Emails
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Email(new Domain("tesla.com"), "no-reply@tesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Email(new Domain("tesla2.com"), "no-reply@tesla2.com")))));

        //subdomain
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("xyz.tesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Domain("xyz.tesla2.com")))));

        // DNS records
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DNSRecord(DnsRecordType.A, "xyz.tesla.com", "1.3.3.7")))));
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DNSRecord(DnsRecordType.A, "example.com", "172.16.17.15")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DNSRecord(DnsRecordType.A, "example.com", "172.16.16.15")))));

        // test for inscope host from domain relationship
        var processor = AssetProcessorFactory.Create(new MockTaskQueueService());
        await processor.ProcessAsync("xyz.tesla.com IN A 1.3.3.7");
        var host = context.Hosts.First(h => h.IP == "1.3.3.7");
        Assert.True(host.InScope);
    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        var processor = AssetProcessorFactory.Create(new MockTaskQueueService());

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
        await processor.ProcessAsync(Serializer.Instance.Serialize(exampleUrl));

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
    public async Task AssetRepository_Tests()
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
    public async Task AssetProcessor_Tests()
    {
        var processor = AssetProcessorFactory.Create(new MockTaskQueueService());
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        await processor.ProcessAsync("tesla.com");

        var domain = context.Domains.First(d => d.Name == "tesla.com");
        Assert.True(domain.InScope);

        var keyword = context.Keywords.First(d => d.Word == "tesla");
        Assert.True(keyword.InScope);
        var cloudEnumTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await processor.ProcessAsync("tesla.com IN A 31.3.3.7");

        var record = context.DNSRecords.First(r => r.Key == "tesla.com" && r.Value == "31.3.3.7");
        Assert.True(record.InScope);

        var host = context.Hosts.Include(h => h.AARecords).First(host => host.IP == "31.3.3.7");
        Assert.True(host.InScope);
        host.AARecords.Add(record);
        Assert.NotNull(host.AARecords.First());
        Assert.True(host.AARecords.First().InScope);
        Assert.True(host.AARecords.First().Domain.InScope);
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(host))));

        await processor.ProcessAsync("85.25.105.204:65530");
        host = context.Hosts.First(h => h.IP == "85.25.105.204");
        var service = context.Services.First(srv => srv.Origin == "tcp://85.25.105.204:65530");
    }

    [Fact]
    public async Task Tagging_Tests()
    {
        var processor = AssetProcessorFactory.Create(new MockTaskQueueService());
        PwnctlDbContext context = new();

        var exampleUrl = new AssetDTO {
            Asset = "https://example.com",
            Tags = new Dictionary<string,object>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        Asset[] assets = AssetParser.Parse(Serializer.Instance.Serialize(exampleUrl), out Type[] assetTypes);
        await processor.ProcessAsync(Serializer.Instance.Serialize(exampleUrl));

        var endpoint = (Endpoint)assets.First(a => a.GetType() == typeof(Endpoint));

        var ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        exampleUrl.Tags = new Dictionary<string, object> {
            {"Server", "apache"},   // testing that existing tags don't get updated
            {"newTag", "whatever"}, // testing that new tags are added to existing assets
            {"emptyTag", ""}        // testing that empty tags are not added
        };

        await processor.ProcessAsync(Serializer.Instance.Serialize(exampleUrl));

        endpoint = context.Endpoints.Include(e => e.Tags).Where(t => t.Url == "https://example.com:443/").First();

        srvTag = endpoint.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        var newTag = endpoint.Tags.First(t => t.Name == "newtag");
        Assert.Equal("whatever", newTag.Value);

        Assert.Null(endpoint.Tags.FirstOrDefault(t => t.Name == "emptytag"));

        ctTag = endpoint.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpoint.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

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
        await processor.ProcessAsync(Serializer.Instance.Serialize(teslaUrl));
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
        await processor.ProcessAsync(Serializer.Instance.Serialize(apacheTeslaUrl));
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

        await processor.ProcessAsync(Serializer.Instance.Serialize(sshService));
        var service = context.Services.Where(ep => ep.Origin == "tcp://1.3.3.7:22").First();
        Assert.Equal("ssh", service.ApplicationProtocol);
    }

    [Fact]
    public Task TaskDefinition_Tests()
    {
        // TODO: validate task-definitions.yml/notification-rules.yml (deserialization/Filter/Template Interpolation)
        return Task.CompletedTask;
    }
}