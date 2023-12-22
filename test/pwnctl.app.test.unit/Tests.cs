namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
using pwnctl.app.Assets;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;

using Xunit;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using pwnctl.kernel;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations;
using pwnctl.app.Operations.Enums;
using System.Reflection;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
        PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
        PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
        PwnInfraContextInitializer.Register<CommandExecutor, BashCommandExecutor>();
    }

    [Fact]
    public void AssetParser_Tests()
    {
        var result = AssetParser.Parse("example.com");
        var domain = result.Value as DomainName;
        Assert.IsType<DomainName>(domain);
        Assert.Equal("example.com", domain.Name);

        // parent domain parsing test
        result = AssetParser.Parse("multi.level.sub.example.com");
        domain = result.Value as DomainName;
        Assert.IsType<DomainName>(domain);
        Assert.Equal("multi.level.sub.example.com", domain.ToString());
        Assert.Equal("level.sub.example.com", domain.ParentDomain.ToString());
        Assert.Equal("sub.example.com", domain.ParentDomain.ParentDomain.ToString());
        Assert.Equal("example.com", domain.ParentDomain.ParentDomain.ParentDomain.ToString());
        Assert.Equal("example", domain.Word);

        // fqdn parsing test
        result = AssetParser.Parse("fqdn.example.com.");
        domain = result.Value as DomainName;
        Assert.IsType<DomainName>(domain);
        Assert.Equal("fqdn.example.com", domain.ToString());
        Assert.Equal("example.com", domain.ParentDomain.ToString());
        Assert.Equal("example", domain.Word);

        result = AssetParser.Parse("_dmarc.00example.com");
        domain = (DomainName)result.Value;
        Assert.IsType<DomainName>(domain);
        Assert.Equal("_dmarc.00example.com", domain.ToString());
        Assert.Equal("00example.com", domain.ParentDomain.ToString());

        // host
        result = AssetParser.Parse("1.3.3.7");
        var host = result.Value as NetworkHost;
        Assert.IsType<NetworkHost>(host);
        Assert.Equal("1.3.3.7", host.IP);

        //ipv6 parsing
        result = AssetParser.Parse("FD00:DEAD:BEEF:64:35::2");
        host = result.Value as NetworkHost;
        Assert.IsType<NetworkHost>(host);
        Assert.Equal("fd00:dead:beef:64:35::2", host.ToString());

        // service
        result = AssetParser.Parse("76.24.104.208:65533");
        var service = result.Value as NetworkSocket;
        Assert.IsType<NetworkSocket>(service);
        Assert.NotNull(service.NetworkHost);
        Assert.Equal("tcp://76.24.104.208:65533", service.ToString());
        Assert.Equal(65533, service.Port);

        // ipv6 parsing
        result = AssetParser.Parse("[FD00:DEAD:BEEF:64:35::2]:163");
        service = result.Value as NetworkSocket;
        Assert.NotNull(service);
        Assert.NotNull(service.NetworkHost);
        Assert.Equal("tcp://fd00:dead:beef:64:35::2:163", service.ToString()); // TODO: <-- add [ ] arount ipv6
        Assert.Equal(163, service.Port);
        Assert.Equal(TransportProtocol.TCP, service.TransportProtocol);

        // transport protocol parsing test
        result = AssetParser.Parse("udp://76.24.104.208:161");
        service = result.Value as NetworkSocket;
        Assert.NotNull(service);
        Assert.NotNull(service.NetworkHost);
        Assert.Equal(TransportProtocol.UDP, service.TransportProtocol);
        Assert.Equal(161, service.Port);
        Assert.Equal("udp://76.24.104.208:161", service.ToString());

        // netrange
        result = AssetParser.Parse("172.16.17.0/24");
        var netRange = result.Value as NetworkRange;
        Assert.NotNull(netRange);
        Assert.Equal("172.16.17.0", netRange.FirstAddress);
        Assert.Equal(24, netRange.NetPrefixBits);
        Assert.Equal("172.16.17.0/24", netRange.ToString());

        // ipv6 parsing
        result = AssetParser.Parse("2001:db8::/48");
        netRange = result.Value as NetworkRange;
        Assert.NotNull(netRange);
        Assert.Equal("2001:db8::", netRange.FirstAddress);
        Assert.Equal(48, netRange.NetPrefixBits);
        Assert.Equal("2001:db8::/48", netRange.ToString());

        // dns record
        result = AssetParser.Parse("xyz.example.com IN A 31.3.3.7");
        var record = result.Value as DomainNameRecord;
        Assert.NotNull(record);
        Assert.Equal("xyz.example.com", record.DomainName.ToString());
        Assert.Equal("31.3.3.7", record.NetworkHost.ToString());
        Assert.Equal("A", record.Type.ToString());
        Assert.Equal("xyz.example.com IN A 31.3.3.7", record.ToString());

        result = AssetParser.Parse("zzz.example.com      IN         A            31.3.3.8");
        record = result.Value as DomainNameRecord;
        Assert.NotNull(record);
        Assert.Equal("zzz.example.com", record.DomainName.ToString());
        Assert.Equal("31.3.3.8", record.NetworkHost.ToString());
        Assert.Equal("A", record.Type.ToString());
        Assert.Equal("zzz.example.com IN A 31.3.3.8", record.ToString());

        result = AssetParser.Parse("8.8.8.8.in-addr.arpa IN PTR dns.google.");
        record = result.Value as DomainNameRecord;
        Assert.NotNull(record);
        Assert.Equal("8.8.8.8.in-addr.arpa", record.DomainName.ToString());
        Assert.Equal("dns.google.", record.Value);
        Assert.Equal("PTR", record.Type.ToString());
        Assert.Equal("8.8.8.8.in-addr.arpa IN PTR dns.google.", record.ToString());

        result = AssetParser.Parse("tesla.com.  IN SOA dns1.p05.nsone.net. hostmaster.nsone.net. 1647625169 43200 7200 1209600 3600q");
        record = result.Value as DomainNameRecord;
        Assert.NotNull(record);
        Assert.Equal("tesla.com", record.DomainName.Name);
        Assert.Equal("SOA", record.Type.ToString());
        Assert.Equal("dns1.p05.nsone.net. hostmaster.nsone.net. 1647625169 43200 7200 1209600 3600q", record.Value);
        Assert.Equal("tesla.com IN SOA dns1.p05.nsone.net. hostmaster.nsone.net. 1647625169 43200 7200 1209600 3600q", record.ToString());
        Assert.Null(record.NetworkHost);

        // spf record parsing
        var spfRecord = "tesla.com IN TXT \"v=spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net -all\"";
        result = AssetParser.Parse(spfRecord);
        Assert.IsType<DomainNameRecord>(result.Value);
        Assert.Equal(3, ((DomainNameRecord)result.Value).SPFHosts.Count());
        Assert.NotNull(((DomainNameRecord)result.Value).DomainName);

        //endpoint
        // subdirectory parsing test
        result = AssetParser.Parse("https://xyz.example.com:8443/api/token");
        Assert.IsType<HttpEndpoint>(result.Value);
        Assert.NotNull(((HttpEndpoint)result.Value).Socket);
        Assert.NotNull(((HttpEndpoint)result.Value).ParentEndpoint);

        // protocol relative url parsing
        result = AssetParser.Parse("//prurl.example.com/test");
        Assert.IsType<HttpEndpoint>(result.Value);

        // TODO: UNC parsing
        // asset = AssetParser.Parse("\\unc.example.com:8443");
        // Assert.IsType<HttpEndpoint>(asset.Value);

        // parameter
        result = AssetParser.Parse("https://xyz.example.com:8443/api/token?_u=xxx&second=");
        Assert.IsType<HttpEndpoint>(result.Value);
        Assert.Equal(2, ((HttpEndpoint)result.Value).HttpParameters.Count);

        // ipv6 parsing
        result = AssetParser.Parse("http://[FD00:DEAD:BEEF:64:35::2]:80/ipv6test");
        Assert.IsType<HttpEndpoint>(result.Value);
        Assert.NotNull(((HttpEndpoint)result.Value).Socket);

        // email
        result = AssetParser.Parse("no-reply@tesla.com");
        Assert.IsType<Email>(result.Value);
        Assert.NotNull(((Email)result.Value).DomainName);

        // mailto: parsing test
        result = AssetParser.Parse("mailto:test@tesla.com");
        Assert.IsType<Email>(result.Value);
        Assert.NotNull(((Email)result.Value).DomainName);

        // maito: parsing test
        result = AssetParser.Parse("maito:test@tesla.com");
        Assert.IsType<Email>(result.Value);
        Assert.NotNull(((Email)result.Value).DomainName);
    }

    [Fact]
    public void ScopeChecking_Tests()
    {
        // net range
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(NetworkRange.TryParse("172.16.17.0/24").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(NetworkRange.TryParse("172.16.16.0/24").Value));

        // host in netrange
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(NetworkHost.TryParse("172.16.17.4").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(NetworkHost.TryParse("172.16.16.5").Value));

        // endpoint in net range
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(HttpEndpoint.TryParse("https://172.16.17.15:443/api/token").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(HttpEndpoint.TryParse("https://172.16.16.15:443/api/token").Value));

        // domain
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("tesla.com").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("tttesla.com").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("tesla.com.net").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("tesla2.com").Value));

        //subdomain
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("xyz.tesla.com").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainName.TryParse("xyz.tesla2.com").Value));
        
        // Emails
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(Email.TryParse("no-reply@tesla.com").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(Email.TryParse("no-reply@tesla2.com").Value));

        // DNS records
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainNameRecord.TryParse("xyz.tesla.com IN A 1.3.3.7").Value));
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainNameRecord.TryParse("example.com IN A 172.16.17.15").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(DomainNameRecord.TryParse("example.com IN A 172.16.16.15").Value));
    }

    [Fact]
    public async Task AssetRepository_Tests()
    {
        PwnctlDbContext context = new();
        AssetDbRepository repository = new();

        var inScopeDomain = DomainName.TryParse("tesla.com").Value;
        var outOfScope = DomainName.TryParse("www.outofscope.com").Value;

        Assert.Null(repository.FindMatching(inScopeDomain));
        await repository.SaveAsync(new AssetRecord(inScopeDomain));
        Assert.NotNull(repository.FindMatching(inScopeDomain));
        inScopeDomain = context.DomainNames.First(d => d.Name == "tesla.com");
        await repository.SaveAsync(new AssetRecord(outOfScope));
        outOfScope = context.DomainNames.First(d => d.Name == "www.outofscope.com");

        var record1 = DomainNameRecord.TryParse("hackerone.com IN A 1.3.3.7").Value;
        var record2 = DomainNameRecord.TryParse("hackerone.com IN AAAA dead:beef::::").Value;

        Assert.Null(repository.FindMatching(record1));
        Assert.Null(repository.FindMatching(record2));
        await repository.SaveAsync(new AssetRecord(record1));
        Assert.NotNull(repository.FindMatching(record1));
        Assert.Null(repository.FindMatching(record2));
        await repository.SaveAsync(new AssetRecord(record2));
        Assert.NotNull(repository.FindMatching(record2));

        var netRange = NetworkRange.TryParse("10.1.101.0/24").Value;
        Assert.Null(repository.FindMatching(netRange));
        await repository.SaveAsync(new AssetRecord(netRange));
        Assert.NotNull(repository.FindMatching(netRange));

        var service = NetworkSocket.TryParse($"{inScopeDomain}:443").Value;
        Assert.Null(repository.FindMatching(service));
        await repository.SaveAsync(new AssetRecord(service));
        Assert.NotNull(repository.FindMatching(service));
    }

    [Fact]
    public async Task Tagging_Tests()
    {
        var proc = new AssetProcessor();
        PwnctlDbContext context = new();
        AssetDbRepository repository = new(context);
        TaskDbRepository taskRepository = new(context);

        var exampleUrl = new AssetDTO
        {
            Asset = "https://example.com",
            Tags = new Dictionary<string, string>{
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

        exampleUrl.Tags = new Dictionary<string, string> {
            {"Server", "apache"},   // testing that existing tags don't get updated
            {"newTag", "whatever"}, // testing that new tags are added to existing assets
            {"emptyTag", ""},        // testing that empty tags are not added
            {"nullTag", ""}
        };

        await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl), EntityFactory.TaskRecord.Id);

        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(t => t.HttpEndpoint.Url == "https://example.com/");

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
        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(ep => ep.HttpEndpoint.Url == "https://iis.tesla.com/");
        var tasks = context.TaskRecords.Include(t => t.Definition).Where(t => t.Record.Id == endpointRecord.Id).ToList();
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
        endpointRecord = repository.ListHttpEndpointsAsync(0).Result.First(r => r.HttpEndpoint.Url == "https://apache.tesla.com/");

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
        var proc = new AssetProcessor();
        PwnctlDbContext context = new();
        TaskDbRepository repository = new(context);

        await proc.ProcessAsync("tesla.com", EntityFactory.TaskRecord.Id);

        var record = context.AssetRecords.First(r => r.DomainName.Name == "tesla.com");
        Assert.True(record.InScope);
        Assert.Equal("tesla", record.DomainName.Word);

        var cloudEnumTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .First(t => t.Definition.Name == ShortName.Create("cloud_enum"));
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await proc.ProcessAsync("tesla.com IN A 31.3.3.7", EntityFactory.TaskRecord.Id);

        record = context.AssetRecords.First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords
                        .Include(r => r.NetworkHost)
                            .ThenInclude(h => h.AARecords)
                            .ThenInclude(h => h.DomainName)
                        .First(r => r.NetworkHost.IP == "31.3.3.7");
        Assert.True(record.InScope);
        Assert.NotNull(record.NetworkHost.AARecords.First());
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions, scope => scope.Definition.Matches(record.NetworkHost));

        record = context.AssetRecords.First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
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
        record = context.AssetRecords.First(r => r.DomainNameRecord.Key == "xyz.tesla.com" && r.DomainNameRecord.Value == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.NetworkHost.IP == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.DomainNameRecord.Value == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.DomainName.Name == "xyz.tesla.com");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.NetworkSocket.Address == "tcp://1.3.3.7:443");
        Assert.False(record.InScope);

        record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://1.3.3.7/");
        Assert.False(record.InScope);

        record = context.AssetRecords.First(r => r.NetworkSocket.Address == "tcp://xyz.tesla.com:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://xyz.tesla.com/");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.HttpParameter.Url == "https://xyz.tesla.com/api" && r.HttpParameter.Name == "key");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.HttpParameter.Url == "https://xyz.tesla.com/api" && r.HttpParameter.Name == "duplicate");
        Assert.True(record.InScope);

        proc = new AssetProcessor();
        await proc.ProcessAsync("https://1.3.3.7:443", EntityFactory.TaskRecord.Id);
        context = new();
        record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://1.3.3.7/");
        Assert.True(record.InScope);
        record = context.AssetRecords.First(r => r.NetworkSocket.Address == "tcp://1.3.3.7:443");
        Assert.True(record.InScope);

        await proc.ProcessAsync("https://abc.tesla.com", EntityFactory.TaskRecord.Id);
        record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://abc.tesla.com/");
        Assert.True(record.InScope);
        var serv = context.NetworkSockets.First(s => s.Address == "tcp://abc.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.First(r => r.Id == serv.Id);
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

        record = context.AssetRecords.First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://qwe.tesla.com:443", record.NetworkSocket.Address);

        await proc.ProcessAsync($$"""{"Asset":"https://qwe.tesla.com","Tags":{"shortname-misconfig":"true"},"FoundBy":"httpx"}""", EntityFactory.TaskRecord.Id);

        var line = "{\"asset\":\"https://vuln.tesla.com/.git/config\",\"tags\":{\"nuclei-68b329da9893e34099c7d8ad5cb9c940\":\"{\\\"template\\\":\\\"git-config\\\",\\\"severity\\\":\\\"medium\\\",\\\"matcher\\\":null,\\\"extracted\\\":null}\"}}";

        await proc.ProcessAsync(line, EntityFactory.TaskRecord.Id);

    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        TaskDbRepository repository = new(context);
        AssetProcessor proc = new();

        await proc.ProcessAsync("172.16.17.0/24", EntityFactory.TaskRecord.Id);
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("nmap_basic")));
        Assert.False(context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
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

        // Task.Command interpolation test
        var hakrawlerTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
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
                                    .First(t => t.Record.DomainName.Name == "sub.tesla.com"
                                             && t.Definition.Name == ShortName.Create("domain_resolution"));
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'", resolutionTask.Command);

        // blacklist test
        Assert.False(context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .Any(t => t.Record.DomainName.Name == "sub.tesla.com"
                                           && t.Definition.Name == ShortName.Create("subfinder")));

        // Keyword test
        var cloudEnumTask = context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Record)
                                    .First(t => t.Definition.Name == ShortName.Create("cloud_enum"));
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await proc.ProcessAsync("https://tesla.s3.amazonaws.com", EntityFactory.TaskRecord.Id);
        var record = context.AssetRecords.First(r => r.HttpEndpoint.Url == "https://tesla.s3.amazonaws.com/");

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
        // await proc.ProcessAsync(PwnInfraContext.Serializer.Serialize(outOfScope), EntityFactory.TaskRecord.Id);
        // var xx = context.AssetRecords
        //                 .Include(r => r.HttpEndpoint)
        //                 .Include(r => r.Tasks)
        //                 .Where(r => r.HttpEndpoint.Url == "https://outofscope.com/api/token")
        //                 .First();
        // Assert.Empty(xx.Tasks);


        // TODO: test TaskDefinition.CheckOutOfScope
        // TODO: test NotificationRule.CheckOutOfScope
    }

    [Fact]
    public async Task OperationManager_Tests()
    {
        PwnctlDbContext context = new();
        AssetProcessor proc = new();
        AssetDbRepository assetRepo = new();
        OperationManager opManager = new(new OperationDbRepository(), new TaskDbRepository(), new EventBridgeClient());

        var op = new Operation("monitor_op", OperationType.Monitor, EntityFactory.Policy, EntityFactory.ScopeAggregate);

        var domain = DomainName.TryParse("deep.sub.tesla.com").Value;
        var record = new AssetRecord(domain);
        var domain2 = DomainName.TryParse("sub.tesla.com").Value;
        var record2 = new AssetRecord(domain2);
        var domain3 = DomainName.TryParse("tesla.com").Value;
        var record3 = new AssetRecord(domain3);
        record.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        record2.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        record3.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        context.Entry(op.Scope).State = EntityState.Unchanged;
        context.Entry(op.Policy).State = EntityState.Unchanged;
        domain3.ParentDomain = null;
        context.Add(record3);
        domain2.ParentDomain = null;
        context.Add(record2);
        domain.ParentDomain = null;
        context.Add(record);
        context.Add(op);
        await context.SaveChangesAsync();

        // Schedule - no previous occurence - added
        await opManager.TryHandleAsync(op.Id);

        // PreCondition tests
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("cloud_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")
                                                                          && t.Record.DomainName.Name == "deep.sub.tesla.com"));

        //  Schedule - previous occurance passed schedule - added
        SystemTime.SetDateTime(DateTime.UtcNow.AddDays(2));
        await opManager.TryHandleAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        //  Schedule - previous occurance not passed schedule - not added
        await opManager.TryHandleAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        // PostCondition & NotificationTemplate tests
        record3.MergeTags(new Dictionary<string, string> { { "rcode", "NXDOMAIN"} }, true);
        await assetRepo.SaveAsync(record3);

        var task = context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
                            .First(t => t.Definition.Name == ShortName.Create("domain_resolution")
                                   && t.Record.DomainName.Name == domain3.Name);

        await proc.TryProcessAsync($$$"""{"Asset":"{{{domain3.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        // check #1 rcode tag got updated
        var rcodeTag = context.Tags
                            .First(t => t.RecordId == record3.Id && t.Name == "rcode");
        Assert.Equal("SERVFAIL", rcodeTag.Value);

        context = new();
        // check #2 notification sent?
        var notification = context.Notifications
                            .Include(n => n.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(r => r.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(t => t.Definition)
                            .First(n => n.TaskId == task.Id && n.Record.DomainName.Name == domain3.Name);
        Assert.Equal(task.Id, notification.TaskId.Value);
        Assert.Equal(record3.Id, notification.RecordId);

        // check #3 NotificationTemplate
        notification.Task = task;
        Assert.Equal("domain tesla.com changed rcode from NXDOMAIN to SERVFAIL", notification.GetText());

        // check #4 new asset notification
        await proc.TryProcessAsync($$$"""{"Asset":"new.{{{domain3.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        var newDomain = context.DomainNames.First(d => d.Name == "new."+ domain3.Name);

        notification = context.Notifications.FirstOrDefault(n => n.TaskId == task.Id && n.RecordId == newDomain.Id);
        Assert.NotNull(notification);

        // check #5 notificationRule.Template test
        var notificationRule = context.NotificationRules.First(n => n.Name == ShortName.Create("mdwfuzzer"));

        await proc.TryProcessAsync($$$"""{"Asset":"https://{{{domain3.Name}}}/","tags":{"mdwfuzzer_check":"uri-override-header"}}""", EntityFactory.TaskRecord.Id);
        var endpoint = context.HttpEndpoints.First(e => e.Url == "https://tesla.com/");
        notification = context.Notifications
                                .Include(n => n.Rule)
                                .Include(n => n.Record)
                                    .ThenInclude(n => n.Tags)
                                .First(n => n.RuleId == notificationRule.Id);
        Assert.Equal("https://tesla.com/ triggered mdwfuzzer check uri-override-header with word ", notification.GetText());

        // TODO: Add Scan Type Operation initialization test
    }

    [Fact]
    public async Task TaskRecord_Tests()
    {
        PwnctlDbContext context = new();
        var proc = new AssetProcessor();
        var taskRepo = new TaskDbRepository(context);
        var executor = new BashCommandExecutor();

        var task = EntityFactory.TaskRecord;

        task.Started();
        await taskRepo.TryUpdateAsync(task);

        (int exitCode, StringBuilder stdout, StringBuilder stderr) = await executor.ExecuteAsync("echo example.com");

        task.Finished(exitCode, stderr.ToString());
        await taskRepo.TryUpdateAsync(task);

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

        (_, stdout, _) = await executor.ExecuteAsync($$"""echo '{"Asset":"example2.com"}'""");

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

        (_, stdout, _) = await executor.ExecuteAsync($$$"""echo '{"Asset":"sub.example3.com","tags":{"test":"tag"}}'""");

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
