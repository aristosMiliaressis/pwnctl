namespace pwnctl.app.test.unit;

using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
using pwnctl.domain.Interfaces;
using pwnctl.app.Assets;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.infra;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Repositories;

using System.Net;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Commands;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_IsTestRun", "true");
        Environment.SetEnvironmentVariable("PWNCTL_InstallPath", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", ".");

        PwnInfraContextInitializer.Setup();

        // reset the database for every test method
        DatabaseInitializer.InitializeAsync().Wait();
    }

    [Fact]
    public void AssetParser_Tests()
    {
        Asset asset = AssetParser.Parse("example.com");
        Assert.IsType<Domain>(asset);
        Assert.NotNull(((Domain)asset).Word);

        // parent domain parsing test
        asset = AssetParser.Parse("multi.level.sub.example.com");
        Assert.IsType<Domain>(asset);

        var domain = (Domain)asset;
        Assert.Equal("multi.level.sub.example.com", domain.Name);
        Assert.Equal("level.sub.example.com", domain.ParentDomain.Name);
        Assert.Equal("sub.example.com", domain.ParentDomain.ParentDomain.Name);
        Assert.Equal("example.com", domain.ParentDomain.ParentDomain.ParentDomain.Name);
        Assert.Equal("example", domain.Word);

        // fqdn parsing test
        asset = AssetParser.Parse("fqdn.example.com.");
        Assert.IsType<Domain>(asset);

        domain = (Domain)asset;
        Assert.Equal("fqdn.example.com", domain.Name);
        Assert.Equal("example.com", domain.ParentDomain.Name);
        Assert.Equal("example", domain.Word);

        // host
        asset = AssetParser.Parse("1.3.3.7");
        Assert.IsType<Host>(asset);

        //ipv6 parsing
        asset = AssetParser.Parse("FD00:DEAD:BEEF:64:35::2");
        Assert.IsType<Host>(asset);

        // service
        asset = AssetParser.Parse("76.24.104.208:65533");
        Assert.IsType<Service>(asset);
        Assert.NotNull(((Service)asset).Host);

        // ipv6 parsing 
        asset = AssetParser.Parse("[FD00:DEAD:BEEF:64:35::2]:163");
        Assert.IsType<Service>(asset);
        Assert.NotNull(((Service)asset).Host);

        // transport protocol parsing test
        asset = AssetParser.Parse("udp://76.24.104.208:161");
        Assert.IsType<Service>(asset);
        Assert.NotNull(((Service)asset).Host);
        Assert.Equal(TransportProtocol.UDP, ((Service)asset).TransportProtocol);
        Assert.Equal(161, ((Service)asset).Port);

        // netrange
        asset = AssetParser.Parse("172.16.17.0/24");
        Assert.IsType<NetRange>(asset);

        // ipv6 parsing
        asset = AssetParser.Parse("2001:db8::/48");
        Assert.IsType<NetRange>(asset);

        // dns record
        asset = AssetParser.Parse("xyz.example.com IN A 31.3.3.7");
        Assert.IsType<DNSRecord>(asset);
        Assert.NotNull(((DNSRecord)asset).Domain);
        Assert.NotNull(((DNSRecord)asset).Host);

        // spf record parsing
        var spfRecord = "tesla.com IN TXT \"v = spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net - all\"";
        asset = AssetParser.Parse(spfRecord);
        Assert.IsType<DNSRecord>(asset);
        Assert.Equal(3, ((DNSRecord)asset).SPFHosts.Count());
        Assert.NotNull(((DNSRecord)asset).Domain);

        //endpoint
        // subdirectory parsing test
        asset = AssetParser.Parse("https://xyz.example.com:8443/api/token");
        Assert.IsType<Endpoint>(asset);
        Assert.NotNull(((Endpoint)asset).Service);
        Assert.NotNull(((Endpoint)asset).ParentEndpoint);

        // TODO: protocol relative url parsing
        // asset = AssetParser.Parse("//prurl.example.com/test");
        // Assert.IsType<Endpoint>(asset);

        // TODO: UNC parsing
        // asset = AssetParser.Parse("\\unc.example.com:8443");
        // Assert.IsType<Endpoint>(asset);

        // parameter
        asset = AssetParser.Parse("https://xyz.example.com:8443/api/token?_u=xxx");
        Assert.IsType<Endpoint>(asset);
        Assert.NotEmpty(((Endpoint)asset).Parameters);

        // ipv6 parsing 
        asset = AssetParser.Parse("http://[FD00:DEAD:BEEF:64:35::2]:80/ipv6test");
        Assert.IsType<Endpoint>(asset);
        Assert.NotNull(((Endpoint)asset).Service);

        // email
        asset = AssetParser.Parse("no-reply@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).Domain);

        // mailto: parsing test
        asset = AssetParser.Parse("mailto:test@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).Domain);

        // maito: parsing test
        asset = AssetParser.Parse("maito:test@tesla.com");
        Assert.IsType<Email>(asset);
        Assert.NotNull(((Email)asset).Domain);

        //NetRagne.RouteTo(ipv4|ipv6)
        //Parameters/VirtualHosts
        // spfv1 vs spfv2?
        // PTR records
    }

    [Fact]
    public void PublicSuffixListService_Tests()
    {
        var exampleDomain = new Domain("xyz.example.com");

        Assert.Equal("example.com", exampleDomain.GetRegistrationDomain());
        Assert.Equal("com", PublicSuffixRepository.Instance.GetSuffix(exampleDomain.Name).Value);

        var exampleSubDomain = new Domain("sub.example.azurewebsites.net");

        Assert.Equal("example.azurewebsites.net", exampleSubDomain.GetRegistrationDomain());
        Assert.Equal("azurewebsites.net", PublicSuffixRepository.Instance.GetSuffix(exampleSubDomain.Name).Value);

        var ep1 = new Endpoint("https", new Service(new Domain("example.com"), 443), "/");
        var ep2 = new Endpoint("https", new Service(new Domain("example.s3.amazonaws.com"), 443), "/");

        Assert.False(CloudServiceRepository.Instance.IsCloudService(ep1));
        Assert.True(CloudServiceRepository.Instance.IsCloudService(ep2));
    }

    [Fact]
    public void ScopeChecking_Tests()
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
    }

    [Fact]
    public async Task AssetRepository_Tests()
    {
        PwnctlDbContext context = new();
        AssetDbRepository repository = new(context);

        var inScopeDomain = new Domain("tesla.com");
        var outOfScope = new Domain("www.outofscope.com");

        Assert.Null(context.FindAsset(inScopeDomain));
        await repository.SaveAsync(new AssetRecord(inScopeDomain));
        Assert.NotNull(context.FindAsset(inScopeDomain));
        inScopeDomain = context.Domains.First(d => d.Name == "tesla.com");
        await repository.SaveAsync(new AssetRecord(outOfScope));
        outOfScope = context.Domains.First(d => d.Name == "www.outofscope.com");

        var record1 = new DNSRecord(DnsRecordType.A, "hackerone.com", "1.3.3.7");
        var record2 = new DNSRecord(DnsRecordType.AAAA, "hackerone.com", "dead:beef::::");

        Assert.Null(context.FindAsset(record1));
        Assert.Null(context.FindAsset(record2));
        await repository.SaveAsync(new AssetRecord(record1));
        Assert.NotNull(context.FindAsset(record1));
        Assert.Null(context.FindAsset(record2));
        await repository.SaveAsync(new AssetRecord(record2));
        Assert.NotNull(context.FindAsset(record2));

        var netRange = new NetRange(System.Net.IPAddress.Parse("10.1.101.0"), 24);
        Assert.Null(context.FindAsset(netRange));
        await repository.SaveAsync(new AssetRecord(netRange));
        Assert.NotNull(context.FindAsset(netRange));

        var service = new Service(inScopeDomain, 443);
        Assert.Null(context.FindAsset(service));
        await repository.SaveAsync(new AssetRecord(service));
        Assert.NotNull(context.FindAsset(service));
    }

    [Fact]
    public async Task AssetProcessor_Tests()
    {
        var processor = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        await processor.ProcessAsync("tesla.com");

        var record = context.AssetRecords.Include(r => r.Domain).First(r => r.Domain.Name == "tesla.com");
        Assert.True(record.InScope);
        Assert.Equal("tesla", record.Domain.Word);

        var cloudEnumTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await processor.ProcessAsync("tesla.com IN A 31.3.3.7");

        record = context.AssetRecords.Include(r => r.DNSRecord).First(r => r.DNSRecord.Key == "tesla.com" && r.DNSRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Host).ThenInclude(h => h.AARecords).First(r => r.Host.IP == "31.3.3.7");
        Assert.True(record.InScope);
        Assert.NotNull(record.Host.AARecords.First());
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(record.Host))));

        record = context.AssetRecords.Include(r => r.DNSRecord).First(r => r.DNSRecord.Key == "tesla.com" && r.DNSRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        await processor.ProcessAsync("6.6.6.6:65530");
        record.Host = context.Hosts.First(h => h.IP == "6.6.6.6");
        var service = context.Services.First(srv => srv.Origin == "tcp://6.6.6.6:65530");

        await processor.TryProcessAsync("sub.tesla.com");
        var domain = context.Domains.First(a => a.Name == "sub.tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);
        domain = context.Domains.First(a => a.Name == "tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);

        await processor.ProcessAsync("https://1.3.3.7:443");
        await processor.ProcessAsync("https://xyz.tesla.com:443");
        await processor.ProcessAsync("https://xyz.tesla.com:443/api");
        await processor.ProcessAsync("xyz.tesla.com. IN A 1.3.3.7");
        record = context.AssetRecords.Include(r => r.DNSRecord).First(r => r.DNSRecord.Key == "xyz.tesla.com" && r.DNSRecord.Value == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Host).First(r => r.Host.IP == "1.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Service).First(r => r.Service.Origin == "tcp://1.3.3.7:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Endpoint).First(r => r.Endpoint.Url == "https://1.3.3.7/");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Service).First(r => r.Service.Origin == "tcp://xyz.tesla.com:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.Endpoint).First(r => r.Endpoint.Url == "https://xyz.tesla.com/");
        Assert.True(record.InScope);

        await processor.ProcessAsync("https://abc.tesla.com");
        record = context.AssetRecords.Include(r => r.Endpoint).First(r => r.Endpoint.Url == "https://abc.tesla.com/");
        Assert.True(record.InScope);
        var serv = context.Services.First(s => s.Origin == "tcp://abc.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.Include(r => r.Service).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://abc.tesla.com:443", record.Service.Origin);

        await processor.ProcessAsync("{\"Asset\":\"https://qwe.tesla.com\",\"FoundBy\":\"httpx\"}");
        serv = context.Services.First(s => s.Origin == "tcp://qwe.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.Include(r => r.Service).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://qwe.tesla.com:443", record.Service.Origin);
    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        var processor = AssetProcessorFactory.Create();

        await processor.ProcessAsync("172.16.17.0/24");
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "nmap_basic"));
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "ffuf_common"));

        var exampleUrl = new
        {
            asset = "https://172.16.17.15/api/token",
            tags = new Dictionary<string, string>{
               {"Content-Type", "text/html"}
            }
        };

        // TaskDefinition.Filter fail test
        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));

        // aggresivness test
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "hakrawler"));
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "sqlmap"));

        // Task.Command interpolation test
        var hakrawlerTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "hakrawler");
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15/api/token", hakrawlerTask.Command);

        // TaskDefinition.Filter pass test
        await processor.ProcessAsync("https://172.16.17.15/");
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "ffuf_common"));

        // Task added on existing asset
        exampleUrl = new
        {
            asset = "https://172.16.17.15/",
            tags = new Dictionary<string, string>{
               {"Protocol", "IIS"}
            }
        };
        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "shortname_scanner"));

        // multiple interpolation test
        await processor.ProcessAsync("sub.tesla.com");
        var resolutionTask = context.JoinedTaskRecordQueryable()
                                    .First(t => t.Record.Domain.Name == "sub.tesla.com" 
                                             && t.Definition.ShortName == "domain_resolution");
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'", resolutionTask.Command);

        // blacklist test
        Assert.False(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "subfinder"));

        // Keyword test
        var cloudEnumTask = context.JoinedTaskRecordQueryable().First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await processor.ProcessAsync("https://tesla.s3.amazonaws.com");
        var record = context.AssetRecords.Include(r => r.Endpoint).First(r => r.Endpoint.Url == "https://tesla.s3.amazonaws.com/");
        Assert.True(context.JoinedTaskRecordQueryable().Any(t => t.Definition.ShortName == "second_order_takeover"));

        // TODO: AllowActive = false test, csv black&whitelist test
        // TODO: test TaskDefinition.MatchOutOfScope
        // TODO: test NotificationRule.CheckOutOfScope
    }

    [Fact]
    public async Task Tagging_Tests()
    {
        var processor = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();
        AssetDbRepository repository = new(context);

        var exampleUrl = new AssetDTO {
            Asset = "https://example.com",
            Tags = new Dictionary<string,object>{
               {"Content-Type", "text/html"},
               {"Status", "200"},
               {"Server", "IIS"}
            }
        };

        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));

        var endpointRecord = context.AssetRecords
                                .Include(r => r.Tags)
                                .Include(r => r.Endpoint)
                                .First(r => r.Endpoint.Url == "https://example.com/");

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

        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));

        endpointRecord = repository.ListEndpointsAsync().Result.First(t => t.Endpoint.Url == "https://example.com/");

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
        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(teslaUrl));
        endpointRecord = repository.ListEndpointsAsync().Result.First(ep => ep.Endpoint.Url == "https://iis.tesla.com/");
        var tasks = context.JoinedTaskRecordQueryable().Where(t => t.Record.Id == endpointRecord.Asset.Id).ToList();
        Assert.True(!tasks.GroupBy(t => t.DefinitionId).Any(g => g.Count() > 1));
        srvTag = endpointRecord.Tags.First(t => t.Name == "protocol");
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
        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(apacheTeslaUrl));
        endpointRecord = repository.ListEndpointsAsync().Result.First(r => r.Endpoint.Url == "https://apache.tesla.com/");
        tasks = context.JoinedTaskRecordQueryable().Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.ShortName == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(sshService));
        var service = context.Services.First(ep => ep.Origin == "tcp://1.3.3.7:22");
        //Assert.Equal("ssh", service.ApplicationProtocol);
    }

    [Fact]
    public async Task TaskEntry_Tests()
    {
        PwnctlDbContext context = new();
        var processor = AssetProcessorFactory.Create();

        var definition = context.TaskDefinitions.FirstOrDefault(t => t.ShortName == "domain_resolution");
        
        var record = new AssetRecord(new Domain("example.com"));
        var task = new TaskEntry(definition, record);

        var rawInputTestCmd = task.WrappedCommand.Replace(task.Command, "echo example.com");

        var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, rawInputTestCmd);

        string? line = "";
        while ((line = process.StandardOutput.ReadLine()) != null)
        {
            await processor.ProcessAsync(line);
        }

        record = context.AssetRecords.Include(r => r.Tags).Include(r => r.Domain).FirstOrDefault(r => r.Domain.Name == "example.com");
        Assert.Equal("domain_resolution", record?.FoundBy);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        var jsonInputTestCmd = task.WrappedCommand.Replace(task.Command, "echo '{\"Asset\":\"example2.com\"}'");

        process = await CommandExecutor.ExecuteAsync("/bin/bash", null, jsonInputTestCmd);

        while ((line = process.StandardOutput.ReadLine()) != null)
        {
            await processor.ProcessAsync(line);
        }

        record = context.AssetRecords.Include(r => r.Tags).Include(r => r.Domain).First(r => r.Domain.Name == "example2.com");
        Assert.Equal("domain_resolution", record?.FoundBy);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        var jsonAltInputTestCmd = task.WrappedCommand.Replace(task.Command, "echo '{\"Asset\":\"sub.example3.com\",\"tags\":{\"test\":\"tag\"}}'");

        process = await CommandExecutor.ExecuteAsync("/bin/bash", null, jsonAltInputTestCmd);

        while ((line = process.StandardOutput.ReadLine()) != null)
        {
            await processor.ProcessAsync(line);
        }

        record = context.AssetRecords.Include(r => r.Tags).Include(r => r.Domain).FirstOrDefault(r => r.Domain.Name == "sub.example3.com");
        Assert.Equal("domain_resolution", record?.FoundBy);
        Assert.Contains("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        record = context.AssetRecords.Include(r => r.Tags).Include(r => r.Domain).FirstOrDefault(r => r.Domain.Name == "example3.com");
        Assert.Equal("domain_resolution", record?.FoundBy);
        Assert.DoesNotContain("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));
    }
}