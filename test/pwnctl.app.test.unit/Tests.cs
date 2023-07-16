namespace pwnctl.app.test.unit;

using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
using pwnctl.app.Assets;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Common.ValueObjects;
using pwnctl.infra;
using pwnctl.infra.Commands;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;

using Xunit;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Operations;
using pwnctl.infra.Queueing;
using pwnctl.kernel;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_USE_SQLITE", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.SetupAsync().Wait();
        DatabaseInitializer.SeedAsync().Wait();
    }

    [Fact]
    public void AssetParser_Tests()
    {
        Asset asset = AssetParser.Parse("example.com");
        Assert.IsType<DomainName>(asset);
        Assert.NotNull(((DomainName)asset).Word);

        // parent domain parsing test
        asset = AssetParser.Parse("multi.level.sub.example.com");
        Assert.IsType<DomainName>(asset);

        var domain = (DomainName)asset;
        Assert.Equal("multi.level.sub.example.com", domain.Name);
        Assert.Equal("level.sub.example.com", domain.ParentDomain.Name);
        Assert.Equal("sub.example.com", domain.ParentDomain.ParentDomain.Name);
        Assert.Equal("example.com", domain.ParentDomain.ParentDomain.ParentDomain.Name);
        Assert.Equal("example", domain.Word);

        // fqdn parsing test
        asset = AssetParser.Parse("fqdn.example.com.");
        Assert.IsType<DomainName>(asset);

        domain = (DomainName)asset;
        Assert.Equal("fqdn.example.com", domain.Name);
        Assert.Equal("example.com", domain.ParentDomain.Name);
        Assert.Equal("example", domain.Word);

        // host
        asset = AssetParser.Parse("1.3.3.7");
        Assert.IsType<NetworkHost>(asset);

        //ipv6 parsing
        asset = AssetParser.Parse("FD00:DEAD:BEEF:64:35::2");
        Assert.IsType<NetworkHost>(asset);

        // service
        asset = AssetParser.Parse("76.24.104.208:65533");
        Assert.IsType<domain.Entities.NetworkSocket>(asset);
        Assert.NotNull(((domain.Entities.NetworkSocket)asset).NetworkHost);

        // ipv6 parsing
        asset = AssetParser.Parse("[FD00:DEAD:BEEF:64:35::2]:163");
        Assert.IsType<domain.Entities.NetworkSocket>(asset);
        Assert.NotNull(((domain.Entities.NetworkSocket)asset).NetworkHost);

        // transport protocol parsing test
        asset = AssetParser.Parse("udp://76.24.104.208:161");
        Assert.IsType<NetworkSocket>(asset);
        Assert.NotNull(((NetworkSocket)asset).NetworkHost);
        Assert.Equal(TransportProtocol.UDP, ((NetworkSocket)asset).TransportProtocol);
        Assert.Equal(161, ((NetworkSocket)asset).Port);

        // netrange
        asset = AssetParser.Parse("172.16.17.0/24");
        Assert.IsType<NetworkRange>(asset);

        // ipv6 parsing
        asset = AssetParser.Parse("2001:db8::/48");
        Assert.IsType<NetworkRange>(asset);

        // dns record
        asset = AssetParser.Parse("xyz.example.com IN A 31.3.3.7");
        Assert.IsType<DomainNameRecord>(asset);
        Assert.NotNull(((DomainNameRecord)asset).DomainName);
        Assert.NotNull(((DomainNameRecord)asset).NetworkHost);

        asset = AssetParser.Parse("zzz.example.com      IN         A            31.3.3.8");
        Assert.IsType<DomainNameRecord>(asset);
        Assert.NotNull(((DomainNameRecord)asset).DomainName);
        Assert.NotNull(((DomainNameRecord)asset).NetworkHost);

        // spf record parsing
        var spfRecord = "tesla.com IN TXT \"v=spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net -all\"";
        asset = AssetParser.Parse(spfRecord);
        Assert.IsType<DomainNameRecord>(asset);
        Assert.Equal(3, ((DomainNameRecord)asset).SPFHosts.Count());
        Assert.NotNull(((DomainNameRecord)asset).DomainName);

        //endpoint
        // subdirectory parsing test
        asset = AssetParser.Parse("https://xyz.example.com:8443/api/token");
        Assert.IsType<HttpEndpoint>(asset);
        Assert.NotNull(((HttpEndpoint)asset).Socket);
        Assert.NotNull(((HttpEndpoint)asset).ParentEndpoint);

        // protocol relative url parsing
        asset = AssetParser.Parse("//prurl.example.com/test");
        Assert.IsType<HttpEndpoint>(asset);

        // TODO: UNC parsing
        // asset = AssetParser.Parse("\\unc.example.com:8443");
        // Assert.IsType<HttpEndpoint>(asset);

        // parameter
        asset = AssetParser.Parse("https://xyz.example.com:8443/api/token?_u=xxx&second=");
        Assert.IsType<HttpEndpoint>(asset);
        Assert.Equal(2, ((HttpEndpoint)asset).HttpParameters.Count);

        // ipv6 parsing
        asset = AssetParser.Parse("http://[FD00:DEAD:BEEF:64:35::2]:80/ipv6test");
        Assert.IsType<HttpEndpoint>(asset);
        Assert.NotNull(((HttpEndpoint)asset).Socket);

        // email
        asset = AssetParser.Parse("no-reply@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).DomainName);

        // mailto: parsing test
        asset = AssetParser.Parse("mailto:test@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).DomainName);

        // maito: parsing test
        asset = AssetParser.Parse("maito:test@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).DomainName);

        // TODO: HttpHosts
        // TODO: PTR records
    }

    [Fact]
    public void ScopeChecking_Tests()
    {
        // net range
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new NetworkRange(System.Net.IPAddress.Parse("172.16.17.0"), 24)));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new NetworkRange(System.Net.IPAddress.Parse("172.16.16.0"), 24)));

        // host in netrange
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new NetworkHost(IPAddress.Parse("172.16.17.4"))));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new NetworkHost(IPAddress.Parse("172.16.16.5"))));

        // endpoint in net range
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new HttpEndpoint("https", new NetworkSocket(new NetworkHost(IPAddress.Parse("172.16.17.15")), 443), "/api/token")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new HttpEndpoint("https", new NetworkSocket(new NetworkHost(IPAddress.Parse("172.16.16.15")), 443), "/api/token")));

        // domain
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("tesla.com")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("tttesla.com")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("tesla.com.net")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("tesla2.com")));

        // Emails
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new Email(new DomainName("tesla.com"), "no-reply@tesla.com")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new Email(new DomainName("tesla2.com"), "no-reply@tesla2.com")));

        //subdomain
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("xyz.tesla.com")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainName("xyz.tesla2.com")));

        // DNS records
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainNameRecord(DnsRecordType.A, "xyz.tesla.com", "1.3.3.7")));
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainNameRecord(DnsRecordType.A, "example.com", "172.16.17.15")));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(new DomainNameRecord(DnsRecordType.A, "example.com", "172.16.16.15")));
    }

    [Fact]
    public async Task AssetRepository_Tests()
    {
        PwnctlDbContext context = new();
        AssetDbRepository repository = new();

        var inScopeDomain = new DomainName("tesla.com");
        var outOfScope = new DomainName("www.outofscope.com");

        Assert.Null(repository.FindMatching(inScopeDomain));
        await repository.SaveAsync(new AssetRecord(inScopeDomain));
        Assert.NotNull(repository.FindMatching(inScopeDomain));
        inScopeDomain = context.DomainNames.First(d => d.Name == "tesla.com");
        await repository.SaveAsync(new AssetRecord(outOfScope));
        outOfScope = context.DomainNames.First(d => d.Name == "www.outofscope.com");

        var record1 = new DomainNameRecord(DnsRecordType.A, "hackerone.com", "1.3.3.7");
        var record2 = new DomainNameRecord(DnsRecordType.AAAA, "hackerone.com", "dead:beef::::");

        Assert.Null(repository.FindMatching(record1));
        Assert.Null(repository.FindMatching(record2));
        await repository.SaveAsync(new AssetRecord(record1));
        Assert.NotNull(repository.FindMatching(record1));
        Assert.Null(repository.FindMatching(record2));
        await repository.SaveAsync(new AssetRecord(record2));
        Assert.NotNull(repository.FindMatching(record2));

        var netRange = new NetworkRange(System.Net.IPAddress.Parse("10.1.101.0"), 24);
        Assert.Null(repository.FindMatching(netRange));
        await repository.SaveAsync(new AssetRecord(netRange));
        Assert.NotNull(repository.FindMatching(netRange));

        var service = new domain.Entities.NetworkSocket(inScopeDomain, 443);
        Assert.Null(repository.FindMatching(service));
        await repository.SaveAsync(new AssetRecord(service));
        Assert.NotNull(repository.FindMatching(service));

        // concurrency test
        List<Task> tasks = new();

        var proc = AssetProcessorFactory.Create();

        var teslaUrl = new
        {
            asset = "https://sub.tesla.com/1/2/3/1",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Protocol", "IIS"},
               { "cors-misconfig", "true" } 
            }
        };

        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(AssetProcessorFactory.Create().ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id));
        tasks.Add(repository.SaveAsync(new AssetRecord(service)));
        tasks.Add(repository.SaveAsync(new AssetRecord(service)));
        tasks.Add(repository.SaveAsync(new AssetRecord(service)));
        tasks.Add(repository.SaveAsync(new AssetRecord(service)));

        Task.WaitAll(tasks.ToArray());
    }

    [Fact]
    public async Task Tagging_Tests()
    {
        var proc = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();
        AssetDbRepository repository = new();
        TaskDbRepository taskRepository = new();

        var exampleUrl = new AssetDTO
        {
            Asset = "https://example.com",
            Tags = new Dictionary<string, object>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        var endpointRecord = context.AssetRecords
                                .Include(r => r.Tags)
                                .Include(r => r.HttpEndpoint)
                                .First(r => r.HttpEndpoint.Url == "https://example.com/");

        var ctTag = endpointRecord.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpointRecord.Tags.First(t => t.Name == "status");
        Assert.Equal("200", stTag.Value);

        var srvTag = endpointRecord.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        exampleUrl.Tags = new Dictionary<string, object> {
            {"Server", "apache"},   // testing that existing tags don't get updated
            {"newTag", "whatever"}, // testing that new tags are added to existing assets
            {"emptyTag", ""},        // testing that empty tags are not added
            {"nullTag", ""}
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        endpointRecord = repository.ListEndpointsAsync(0).Result.First(t => t.HttpEndpoint.Url == "https://example.com/");

        srvTag = endpointRecord.Tags.First(t => t.Name == "server");
        Assert.Equal("IIS", srvTag.Value);

        var newTag = endpointRecord.Tags.First(t => t.Name == "newtag");
        Assert.Equal("whatever", newTag.Value);

        Assert.Null(endpointRecord.Tags.FirstOrDefault(t => t.Name == "emptytag"));
        Assert.Null(endpointRecord.Tags.FirstOrDefault(t => t.Name == "nullTag"));

        ctTag = endpointRecord.Tags.First(t => t.Name == "content-type");
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpointRecord.Tags.First(t => t.Name == "status");
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
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl), EntityFactory.TaskRecord.Id);
        endpointRecord = repository.ListEndpointsAsync(0).Result.First(ep => ep.HttpEndpoint.Url == "https://iis.tesla.com/");
        var tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Asset.Id).ToList();
        Assert.DoesNotContain(tasks.GroupBy(t => t.DefinitionId), g => g.Count() > 1);
        srvTag = endpointRecord.Tags.First(t => t.Name == "protocol");
        Assert.Equal("IIS", srvTag.Value);
        Assert.Contains(tasks, t => t.Definition.Name.Value == "shortname_scanner");

        var apacheTeslaUrl = new
        {
            asset = "https://apache.tesla.com",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "apache"},
               { "cors-misconfig", "true" }
            }
        };

        // test Tag filter
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(apacheTeslaUrl), EntityFactory.TaskRecord.Id);
        endpointRecord = repository.ListEndpointsAsync(0).Result.First(r => r.HttpEndpoint.Url == "https://apache.tesla.com/");

        tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.Name.Value == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(sshService), EntityFactory.TaskRecord.Id);
        var service = context.NetworkSockets.First(ep => ep.Address == "tcp://1.3.3.7:22");
    }

    [Fact]
    public async Task AssetProcessor_Tests()
    {
        var proc = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();
        TaskDbRepository repository = new();

        await proc.ProcessAsync("tesla.com", EntityFactory.TaskRecord.Id);

        var record = context.AssetRecords.Include(r => r.DomainName).First(r => r.DomainName.Name == "tesla.com");
        Assert.True(record.InScope);
        Assert.Equal("tesla", record.DomainName.Word);

        var cloudEnumTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .ThenInclude(t => t.DomainName)
                                    .First(t => t.Definition.Name == ShortName.Create("cloud_enum"));
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await proc.ProcessAsync("tesla.com IN A 31.3.3.7", EntityFactory.TaskRecord.Id);

        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.NetworkHost).ThenInclude(h => h.AARecords).First(r => r.NetworkHost.IP == "31.3.3.7");
        Assert.True(record.InScope);
        Assert.NotNull(record.NetworkHost.AARecords.First());
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions, scope => scope.Definition.Matches(record.NetworkHost));

        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        await proc.ProcessAsync("6.6.6.6:65530", EntityFactory.TaskRecord.Id);
        var host = context.NetworkHosts.First(h => h.IP == "6.6.6.6");
        var service = context.NetworkSockets.First(srv => srv.Address == "tcp://6.6.6.6:65530");

        await proc.TryProcessAsync("sub.tesla.com", EntityFactory.TaskRecord.Id);
        var domain = context.DomainNames.First(a => a.Name == "sub.tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);
        domain = context.DomainNames.First(a => a.Name == "tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);

        await proc.ProcessAsync("https://1.3.3.7:443", EntityFactory.TaskRecord.Id);
        await proc.ProcessAsync("https://xyz.tesla.com:443", EntityFactory.TaskRecord.Id);
        await proc.ProcessAsync("https://xyz.tesla.com:443/api?key=xxx", EntityFactory.TaskRecord.Id);
        await proc.ProcessAsync("https://xyz.tesla.com:443/api?duplicate=xxx&duplicate=yyy", EntityFactory.TaskRecord.Id);
        await proc.ProcessAsync("xyz.tesla.com. IN A 1.3.3.7", EntityFactory.TaskRecord.Id);
        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Key == "xyz.tesla.com" && r.DomainNameRecord.Value == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.NetworkHost).First(r => r.NetworkHost.IP == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Value == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.DomainName).First(r => r.DomainName.Name == "xyz.tesla.com");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.NetworkSocket.Address == "tcp://1.3.3.7:443");
        Assert.False(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://1.3.3.7/");
        Assert.False(record.InScope);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.NetworkSocket.Address == "tcp://xyz.tesla.com:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://xyz.tesla.com/");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpParameter).First(r => r.HttpParameter.Url == "https://xyz.tesla.com/api" && r.HttpParameter.Name == "key");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpParameter).First(r => r.HttpParameter.Url == "https://xyz.tesla.com/api" && r.HttpParameter.Name == "duplicate");
        Assert.True(record.InScope);

        proc = AssetProcessorFactory.Create();
        await proc.ProcessAsync("https://1.3.3.7:443", EntityFactory.TaskRecord.Id);
        context = new();
        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.NetworkSocket.Address == "tcp://1.3.3.7:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://1.3.3.7/");
        Assert.True(record.InScope);

        await proc.ProcessAsync("https://abc.tesla.com", EntityFactory.TaskRecord.Id);
        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://abc.tesla.com/");
        Assert.True(record.InScope);
        var serv = context.NetworkSockets.First(s => s.Address == "tcp://abc.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://abc.tesla.com:443", record.NetworkSocket.Address);

        await proc.ProcessAsync($$"""{"Asset":"https://qwe.tesla.com","Tags":{"shortname-misconfig":"true"},"FoundBy":"httpx"}""", EntityFactory.TaskRecord.Id);
        serv = context.NetworkSockets.First(s => s.Address == "tcp://qwe.tesla.com:443");
        Assert.NotNull(serv);

        var endpoint = context.HttpEndpoints.First(s => s.Url == "https://qwe.tesla.com/");
        var notification = context.Notifications.Include(n => n.Rule).First(n => n.Rule.Name == ShortName.Create("shortname_misconfig") && n.RecordId == endpoint.Id);

        await proc.ProcessAsync($$"""{"Asset":"https://qwe.tesla.com","Tags":{"second-order-takeover":"true"},"FoundBy":"httpx"}""", EntityFactory.TaskRecord.Id);
        notification = context.Notifications.Include(n => n.Rule).FirstOrDefault(n => n.Rule.Name == ShortName.Create("second_order_takeover") && n.RecordId == endpoint.Id);
        Assert.Null(notification);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://qwe.tesla.com:443", record.NetworkSocket.Address);
    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        TaskDbRepository repository = new();
        var proc = AssetProcessorFactory.Create();

        await proc.ProcessAsync("172.16.17.0/24", EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("nmap_basic")));
        Assert.False(context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
                                .ThenInclude(r => r.NetworkRange)
                            .Any(t => t.Record.NetworkRange.FirstAddress == "172.16.17.0"
                                   && t.Definition.Name == ShortName.Create("ffuf_common")));

        var exampleUrl = new
        {
            asset = "https://172.16.17.15/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // TaskDefinition.Filter fail test
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        // aggresivness test
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("hakrawler")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sqlmap")));

        // Task.Command interpolation test
        var hakrawlerTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                        .ThenInclude(r => r.HttpEndpoint)
                                    .First(t => t.Definition.Name == ShortName.Create("hakrawler"));
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15/api/token", hakrawlerTask.Command);

        // TaskDefinition.Filter pass test
        await proc.ProcessAsync("https://172.16.17.15/", EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("ffuf_common")));

        // Task added on existing asset
        exampleUrl = new
        {
            asset = "https://172.16.17.15/",
            tags = new Dictionary<string, string>{
               {"Protocol", "IIS"}
            }
        };
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("shortname_scanner")));

        // multiple interpolation test
        await proc.ProcessAsync("sub.tesla.com", EntityFactory.TaskRecord.Id);
        var resolutionTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                        .ThenInclude(r => r.DomainName)
                                    .First(t => t.Record.DomainName.Name == "sub.tesla.com"
                                             && t.Definition.Name == ShortName.Create("domain_resolution"));
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'", resolutionTask.Command);

        // blacklist test
        Assert.False(context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                        .ThenInclude(r => r.DomainName)
                                    .Any(t => t.Record.DomainName.Name == "sub.tesla.com"
                                           && t.Definition.Name == ShortName.Create("subfinder")));

        // Keyword test
        var cloudEnumTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                        .ThenInclude(r => r.DomainName)
                                    .First(t => t.Definition.Name == ShortName.Create("cloud_enum"));
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await proc.ProcessAsync("https://tesla.s3.amazonaws.com", EntityFactory.TaskRecord.Id);
        var record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://tesla.s3.amazonaws.com/");

        var task = context.TaskRecords
                            .Include(t => t.Definition)
                            .First(t => t.Definition.Name == ShortName.Create("second_order_takeover"));
        Assert.NotNull(task);

        var outOfScope = new
        {
            asset = "https://outofscope.com/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // out of scope test
        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(outOfScope), EntityFactory.TaskRecord.Id);
        var xx = context.AssetRecords
                        .Include(r => r.HttpEndpoint)
                        .Include(r => r.Tasks)
                        .Where(r => r.HttpEndpoint.Url == "https://outofscope.com/api/token")
                        .First();
        Assert.Empty(xx.Tasks);


        // TODO: test that crawl mode is not effected by MonitorRules.PreCondition

        // TODO: AllowActive = false test, csv black&whitelist test
        // TODO: test TaskDefinition.MatchOutOfScope
        // TODO: test NotificationRule.CheckOutOfScope
    }

    [Fact]
    public async Task OperationInitializer_Tests()
    {
        PwnctlDbContext context = new();
        var proc = AssetProcessorFactory.Create();
        AssetDbRepository assetRepo = new();
        OperationInitializer initializer = new(new OperationDbRepository(),
                                                new AssetDbRepository(),
                                                new TaskDbRepository(),
                                                TaskQueueServiceFactory.Create());

        var op = new Operation("monitor_op", OperationType.Monitor, EntityFactory.Policy, EntityFactory.ScopeAggregate);

        var domain = new DomainName("deep.sub.tesla.com");
        var record = new AssetRecord(domain);
        var parentDomain = new DomainName("tesla.com");
        var record2 = new AssetRecord(parentDomain);
        record.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        record2.SetScopeId(EntityFactory.ScopeAggregate.Definitions.Last().DefinitionId);
        context.Entry(op.Scope).State = EntityState.Unchanged;
        context.Entry(op.Policy).State = EntityState.Unchanged;
        context.Add(record);
        context.Add(record2);
        context.Add(op);
        await context.SaveChangesAsync();

        // Schedule - no previous occurence - added
        await initializer.InitializeAsync(op.Id);

        // PreCondition tests
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("cloud_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")
                                                                          && t.Record.DomainName.Name == "deep.sub.tesla.com"));

        //  Schedule - previous occurance passed schedule - added
        SystemTime.SetDateTime(DateTime.UtcNow.AddDays(2));
        await initializer.InitializeAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        //  Schedule - previous occurance not passed schedule - not added
        await initializer.InitializeAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        // PostCondition & NotificationTemplate tests
        record2.MergeTags(new Dictionary<string, object> { { "rcode", "NXDOMAIN"} }, true);
        await assetRepo.SaveAsync(record2);

        var task = context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
                                .ThenInclude(r => r.DomainName)
                            .First(t => t.Definition.Name == ShortName.Create("domain_resolution")
                                   && t.Record.DomainName.Name == parentDomain.Name);

        await proc.TryProcessAsync($$$"""{"Asset":"{{{parentDomain.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        // check #1 rcode tag got updated
        var rcodeTag = context.Tags
                            .First(t => t.RecordId == record2.Id && t.Name == "rcode");
        Assert.Equal("SERVFAIL", rcodeTag.Value);

        context = new();
        // check #2 notification sent?
        var notification = context.Notifications
                            .Include(n => n.Record)
                                .ThenInclude(r => r.DomainName)
                            .Include(n => n.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(r => r.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(t => t.Definition)
                            .First(n => n.TaskId == task.Id && n.Record.DomainName.Name == parentDomain.Name);
        Assert.Equal(task.Id, notification.TaskId.Value);
        Assert.Equal(record2.Id, notification.RecordId);

        // check #3 NotificationTemplate
        notification.Task = task;
        Assert.Equal("domain tesla.com changed rcode from NXDOMAIN to SERVFAIL", notification.GetText());

        // check #4 new asset notification
        await proc.TryProcessAsync($$$"""{"Asset":"new.{{{parentDomain.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        var newDomain = context.DomainNames.First(d => d.Name == "new."+parentDomain.Name);

        notification = context.Notifications.FirstOrDefault(n => n.TaskId == task.Id && n.RecordId == newDomain.Id);
        Assert.NotNull(notification);

        // check #5 notificationRule.Template test
        var notificationRule = context.NotificationRules.First(n => n.Name == ShortName.Create("mdwfuzzer"));

        await proc.TryProcessAsync($$$"""{"Asset":"https://{{{parentDomain.Name}}}/","tags":{"mdwfuzzer_check":"uri-override-header"}}""", EntityFactory.TaskRecord.Id);
        var endpoint = context.HttpEndpoints.First(e => e.Url == "https://tesla.com/");
        notification = context.Notifications
                                .Include(n => n.Rule)
                                .Include(n => n.Record)
                                    .ThenInclude(n => n.Tags)
                                .Include(n => n.Record)
                                    .ThenInclude(n => n.HttpEndpoint)
                                .First(n => n.RuleId == notificationRule.Id);
        Assert.Equal("https://tesla.com/ triggered mdwfuzzer check uri-override-header with word ", notification.GetText());
    }

    [Fact]
    public async Task TaskRecord_Tests()
    {
        PwnctlDbContext context = new();
        var proc = AssetProcessorFactory.Create();
        var taskRepo = new TaskDbRepository();

        var task = EntityFactory.TaskRecord;

        task.Started();
        await taskRepo.UpdateAsync(task);

        (int exitCode, StringBuilder stdout, StringBuilder stderr) = await CommandExecutor.ExecuteAsync("echo example.com");

        task.Finished(exitCode, stderr.ToString());
        await taskRepo.UpdateAsync(task);

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await proc.ProcessAsync(line, task.Id);
        }

        var record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "example.com");

        Assert.Equal(task.Definition.Name, record?.FoundByTask.Definition.Name);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        (_, stdout, _) = await CommandExecutor.ExecuteAsync($$"""echo '{"Asset":"example2.com"}'""");

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await proc.ProcessAsync(line, EntityFactory.TaskRecord.Id);
        }

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .First(r => r.DomainName.Name == "example2.com");

        Assert.Equal(EntityFactory.TaskRecord.Definition.Name, record?.FoundByTask.Definition.Name);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        (_, stdout, _) = await CommandExecutor.ExecuteAsync($$$"""echo '{"Asset":"sub.example3.com","tags":{"test":"tag"}}'""");

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await proc.ProcessAsync(line, EntityFactory.TaskRecord.Id);
        }

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "sub.example3.com");

        Assert.Equal(EntityFactory.TaskRecord.Definition.Name, record?.FoundByTask.Definition.Name);
        Assert.Contains("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "example3.com");

        Assert.Equal(EntityFactory.TaskRecord.Definition.Name, record?.FoundByTask.Definition.Name);
        Assert.DoesNotContain("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));
    }
}
