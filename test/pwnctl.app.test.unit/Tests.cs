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
using System.Text;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Debug");

        PwnInfraContextInitializer.Setup();
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
        var spfRecord = "tesla.com IN TXT \"v = spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net - all\"";
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
        asset = AssetParser.Parse("https://xyz.example.com:8443/api/token?_u=xxx&second=test");
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

        //NetworkRagne.RouteTo(ipv4|ipv6)
        //HttpParameters/HttpHosts
        // spfv1 vs spfv2?
        // PTR records
    }

    [Fact]
    public void PublicSuffixListService_Tests()
    {
        var exampleDomain = new DomainName("xyz.example.com");

        Assert.Equal("example.com", exampleDomain.GetRegistrationDomain());
        Assert.Equal("com", PublicSuffixRepository.Instance.GetSuffix(exampleDomain.Name).Value);

        var exampleSubDomain = new DomainName("sub.example.azurewebsites.net");

        Assert.Equal("example.azurewebsites.net", exampleSubDomain.GetRegistrationDomain());
        Assert.Equal("azurewebsites.net", PublicSuffixRepository.Instance.GetSuffix(exampleSubDomain.Name).Value);

        var ep1 = new HttpEndpoint("https", new NetworkSocket(new DomainName("example.com"), 443), "/");
        var ep2 = new HttpEndpoint("https", new NetworkSocket(new DomainName("example.s3.amazonaws.com"), 443), "/");

        Assert.False(CloudServiceRepository.Instance.IsCloudService(ep1));
        Assert.True(CloudServiceRepository.Instance.IsCloudService(ep2));
    }

    [Fact]
    public void ScopeChecking_Tests()
    {
        PwnctlDbContext context = new();

        var programs = context.ListPrograms();

        // net range
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetworkRange(System.Net.IPAddress.Parse("172.16.17.0"), 24)))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetworkRange(System.Net.IPAddress.Parse("172.16.16.0"), 24)))));

        // host in netrange
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetworkHost(IPAddress.Parse("172.16.17.4"))))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new NetworkHost(IPAddress.Parse("172.16.16.5"))))));

        // endpoint in net range
        Assert.NotNull(programs.FirstOrDefault<Scope.Entities.Program>(program => program.Scope.Any<Scope.Entities.ScopeDefinition>(scope => scope.Matches(new HttpEndpoint("https", new NetworkSocket(new NetworkHost(IPAddress.Parse("172.16.17.15")), 443), "/api/token")))));
        Assert.Null(programs.FirstOrDefault<Scope.Entities.Program>(program => program.Scope.Any<Scope.Entities.ScopeDefinition>(scope => scope.Matches(new HttpEndpoint("https", new NetworkSocket(new NetworkHost(IPAddress.Parse("172.16.16.15")), 443), "/api/token")))));

        // domain
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("tesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("tttesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("tesla.com.net")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("tesla2.com")))));

        // Emails
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Email(new DomainName("tesla.com"), "no-reply@tesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new Email(new DomainName("tesla2.com"), "no-reply@tesla2.com")))));

        //subdomain
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("xyz.tesla.com")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainName("xyz.tesla2.com")))));

        // DNS records
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainNameRecord(DnsRecordType.A, "xyz.tesla.com", "1.3.3.7")))));
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainNameRecord(DnsRecordType.A, "example.com", "172.16.17.15")))));
        Assert.Null(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(new DomainNameRecord(DnsRecordType.A, "example.com", "172.16.16.15")))));
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
        inScopeDomain = context.Domains.First(d => d.Name == "tesla.com");
        await repository.SaveAsync(new AssetRecord(outOfScope));
        outOfScope = context.Domains.First(d => d.Name == "www.outofscope.com");

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
    }

    [Fact]
    public async Task AssetProcessor_Tests()
    {
        var processor = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();
        TaskDbRepository repository = new();

        var programs = context.ListPrograms();

        await processor.ProcessAsync("tesla.com");

        var record = context.AssetRecords.Include(r => r.DomainName).First(r => r.DomainName.Name == "tesla.com");
        Assert.True(record.InScope);
        Assert.Equal("tesla", record.DomainName.Word);

        var cloudEnumTask = repository.JoinedQueryable.First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        cloudEnumTask.Queued();
        await repository.UpdateAsync(cloudEnumTask);

        await processor.ProcessAsync("tesla.com IN A 31.3.3.7");

        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.NetworkHost).ThenInclude(h => h.AARecords).First(r => r.NetworkHost.IP == "31.3.3.7");
        Assert.True(record.InScope);
        Assert.NotNull(record.NetworkHost.AARecords.First());
        Assert.NotNull(programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(record.NetworkHost))));

        record = context.AssetRecords.Include(r => r.DomainNameRecord).First(r => r.DomainNameRecord.Key == "tesla.com" && r.DomainNameRecord.Value == "31.3.3.7");
        Assert.True(record.InScope);

        await processor.ProcessAsync("6.6.6.6:65530");
        record.NetworkHost = context.Hosts.First(h => h.IP == "6.6.6.6");
        var service = context.Sockets.First(srv => srv.Address == "tcp://6.6.6.6:65530");

        await processor.TryProcessAsync("sub.tesla.com");
        var domain = context.Domains.First(a => a.Name == "sub.tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);
        domain = context.Domains.First(a => a.Name == "tesla.com");
        context.AssetRecords.First(a => a.Id == domain.Id);

        await processor.ProcessAsync("https://1.3.3.7:443");
        await processor.ProcessAsync("https://xyz.tesla.com:443");
        await processor.ProcessAsync("https://xyz.tesla.com:443/api?key=xxx");
        await processor.ProcessAsync("xyz.tesla.com. IN A 1.3.3.7");
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

        processor = AssetProcessorFactory.Create();
        await processor.ProcessAsync("https://1.3.3.7:443");
        context = new();
        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.NetworkSocket.Address == "tcp://1.3.3.7:443");
        Assert.True(record.InScope);

        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://1.3.3.7/");
        Assert.True(record.InScope);

        await processor.ProcessAsync("https://abc.tesla.com");
        record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://abc.tesla.com/");
        Assert.True(record.InScope);
        var serv = context.Sockets.First(s => s.Address == "tcp://abc.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://abc.tesla.com:443", record.NetworkSocket.Address);

        await processor.ProcessAsync("{\"Asset\":\"https://qwe.tesla.com\",\"FoundBy\":\"httpx\"}");
        serv = context.Sockets.First(s => s.Address == "tcp://qwe.tesla.com:443");
        Assert.NotNull(serv);

        record = context.AssetRecords.Include(r => r.NetworkSocket).First(r => r.Id == serv.Id);
        Assert.True(record.InScope);
        Assert.Equal("tcp://qwe.tesla.com:443", record.NetworkSocket.Address);
    }

    [Fact]
    public async Task TaskFiltering_Tests()
    {
        PwnctlDbContext context = new();
        TaskDbRepository repository = new();
        var processor = AssetProcessorFactory.Create();

        await processor.ProcessAsync("172.16.17.0/24");
        Assert.True(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "nmap_basic"));
        Assert.False(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "ffuf_common"));

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
        Assert.True(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "hakrawler"));
        Assert.False(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "sqlmap"));

        // Task.Command interpolation test
        var hakrawlerTask = repository.JoinedQueryable.First(t => t.Definition.ShortName == "hakrawler");
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15/api/token", hakrawlerTask.Command);

        // TaskDefinition.Filter pass test
        await processor.ProcessAsync("https://172.16.17.15/");
        Assert.True(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "ffuf_common"));

        // Task added on existing asset
        exampleUrl = new
        {
            asset = "https://172.16.17.15/",
            tags = new Dictionary<string, string>{
               {"Protocol", "IIS"}
            }
        };
        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));
        Assert.True(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "shortname_scanner"));

        // multiple interpolation test
        await processor.ProcessAsync("sub.tesla.com");
        var resolutionTask = repository.JoinedQueryable
                                    .First(t => t.Record.DomainName.Name == "sub.tesla.com"
                                             && t.Definition.ShortName == "domain_resolution");
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'", resolutionTask.Command);

        // blacklist test
        Assert.False(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "subfinder"));

        // Keyword test
        var cloudEnumTask = repository.JoinedQueryable.First(t => t.Definition.ShortName == "cloud_enum");
        Assert.Equal("cloud-enum.sh tesla", cloudEnumTask.Command);

        await processor.ProcessAsync("https://tesla.s3.amazonaws.com");
        var record = context.AssetRecords.Include(r => r.HttpEndpoint).First(r => r.HttpEndpoint.Url == "https://tesla.s3.amazonaws.com/");
        Assert.True(repository.JoinedQueryable.Any(t => t.Definition.ShortName == "second_order_takeover"));

        // TODO: AllowActive = false test, csv black&whitelist test
        // TODO: test TaskDefinition.MatchOutOfScope
        // TODO: test NotificationRule.CheckOutOfScope
    }

    [Fact]
    public async Task Tagging_Tests()
    {
        var processor = AssetProcessorFactory.Create();
        PwnctlDbContext context = new();
        AssetDbRepository repository = new();
        TaskDbRepository taskRepository = new();

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

        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(exampleUrl));

        endpointRecord = repository.ListEndpointsAsync().Result.First(t => t.HttpEndpoint.Url == "https://example.com/");

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
        endpointRecord = repository.ListEndpointsAsync().Result.First(ep => ep.HttpEndpoint.Url == "https://iis.tesla.com/");
        var tasks = taskRepository.JoinedQueryable.Where(t => t.Record.Id == endpointRecord.Asset.Id).ToList();
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
        endpointRecord = repository.ListEndpointsAsync().Result.First(r => r.HttpEndpoint.Url == "https://apache.tesla.com/");

        tasks = taskRepository.JoinedQueryable.Where(t => t.Record.Id == endpointRecord.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.ShortName == "shortname_scanner");

        var sshService = new
        {
            asset = "1.3.3.7:22",
            tags = new Dictionary<string, string>{
               {"ApplicationProtocol", "ssh"}
            }
        };

        await processor.ProcessAsync(PwnInfraContext.Serializer.Serialize(sshService));
        var service = context.Sockets.First(ep => ep.Address == "tcp://1.3.3.7:22");
        //Assert.Equal("ssh", service.ApplicationProtocol);
    }

    [Fact]
    public async Task TaskEntry_Tests()
    {
        PwnctlDbContext context = new();
        var processor = AssetProcessorFactory.Create();

        var definition = context.TaskDefinitions.FirstOrDefault(t => t.ShortName == "domain_resolution");

        var record = new AssetRecord(new DomainName("example.com.cn"));
        var task = new TaskEntry(definition, record);
        context.TaskEntries.Add(task);
        context.SaveChanges();

        (_, StringBuilder stdout, _) = await CommandExecutor.ExecuteAsync("echo example.com");

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await processor.ProcessAsync(line, task);
        }

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "example.com");

        Assert.Equal("domain_resolution", record?.FoundByTask.Definition.ShortName);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        (_, stdout, _) = await CommandExecutor.ExecuteAsync("echo '{\"Asset\":\"example2.com\"}'");

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await processor.ProcessAsync(line, task);
        }

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .First(r => r.DomainName.Name == "example2.com");

        Assert.Equal("domain_resolution", record?.FoundByTask.Definition.ShortName);
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        (_, stdout, _) = await CommandExecutor.ExecuteAsync("echo '{\"Asset\":\"sub.example3.com\",\"tags\":{\"test\":\"tag\"}}'");

        foreach (var line in stdout.ToString().Split("\n").Where(l => !string.IsNullOrEmpty(l)))
        {
            await processor.ProcessAsync(line, task);
        }

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "sub.example3.com");

        Assert.Equal("domain_resolution", record?.FoundByTask.Definition.ShortName);
        Assert.Contains("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));

        record = context.AssetRecords
                        .Include(r => r.Tags)
                        .Include(r => r.FoundByTask)
                            .ThenInclude(r => r.Definition)
                        .Include(r => r.DomainName)
                        .FirstOrDefault(r => r.DomainName.Name == "example3.com");

        Assert.Equal("domain_resolution", record?.FoundByTask.Definition.ShortName);
        Assert.DoesNotContain("test", record?.Tags.Select(t => t.Name));
        Assert.DoesNotContain("foundby", record?.Tags.Select(t => t.Name));
    }
}