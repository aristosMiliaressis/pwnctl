namespace pwnctl.tests;

using pwnctl.app.Repositories;
using pwnctl.app.Utilities;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.core.Entities.Assets;
using pwnctl.core.BaseClasses;
using pwnctl.core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNTAINER_TEST", "true");
        Environment.SetEnvironmentVariable("INSTALL_PATH", ".");
        PwnctlDbContext.Initialize();
        PwnctlCoreShim.PublicSuffixRepository = CachedPublicSuffixRepository.Singleton;

        var psi = new ProcessStartInfo();
        psi.FileName = "/bin/bash";
        psi.Arguments = " -c scripts/get_public_suffixes.sh";
        psi.EnvironmentVariables["INSTALL_PATH"] = ".";
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
        Assert.Equal(typeof(Domain), assetTypes[0]);
        Assert.NotNull(assets[0]);
        Assert.Equal(typeof(Domain), assets[0].GetType());

        AssetParser.TryParse("1.3.3.7", out assetTypes, out assets);
        Assert.Equal(typeof(Host), assetTypes[0]);
        Assert.NotNull(assets[0]);
        Assert.Equal(typeof(Host), assets[0].GetType());

        AssetParser.TryParse("8.8.8.8:53", out assetTypes, out assets);
        var service = assets.First(a => a.GetType() == typeof(Service));
        var type = assetTypes.First(t => t == typeof(Service));
        Assert.NotNull(service);
        Assert.Equal(typeof(Service), service.GetType());
        var host = assets.First(a => a.GetType() == typeof(Host));
        type = assetTypes.First(t => t == typeof(Host));
        Assert.NotNull(host);
        Assert.Equal(typeof(Host), host.GetType());

        AssetParser.TryParse("172.16.17.0/24", out assetTypes, out assets);
        Assert.Equal(typeof(NetRange), assetTypes[0]);
        Assert.NotNull(assets[0]);
        Assert.Equal(typeof(NetRange), assets[0].GetType());

        AssetParser.TryParse("xyz.example.com", out assetTypes, out assets);
        Assert.Equal(typeof(Domain), assetTypes[0]);
        Assert.NotNull(assets[0]);
        Assert.Equal(typeof(Domain), assets[0].GetType());

        AssetParser.TryParse("xyz.example.com IN A 31.3.3.7", out assetTypes, out assets);
        var record = assets.First(a => a.GetType() == typeof(DNSRecord));
        var domain = assets.First(a => a.GetType() == typeof(Domain));
        host = assets.First(a => a.GetType() == typeof(Host));
        Assert.Equal(typeof(DNSRecord), assetTypes[0]);
        Assert.NotNull(record);
        Assert.Equal(typeof(DNSRecord), record.GetType());
        Assert.NotNull(host);
        Assert.Equal(typeof(Host), host.GetType());
        Assert.NotNull(domain);
        Assert.Equal(typeof(Domain), domain.GetType());

        AssetParser.TryParse("https://xyz.example.com:8443/api/token", out assetTypes, out assets);
        var endpoint = assets.First(a => a.GetType() == typeof(Endpoint));
        service =  assets.First(a => a.GetType() == typeof(Service));
        domain = assets.First(a => a.GetType() == typeof(Domain));
        Assert.Equal(typeof(Endpoint), assetTypes[0]);
        Assert.NotNull(endpoint);
        Assert.Equal(typeof(Endpoint), endpoint.GetType());
        Assert.NotNull(service);
        Assert.Equal(typeof(Service), service.GetType());
        Assert.NotNull(domain);
        Assert.Equal(typeof(Domain), domain.GetType());

        AssetParser.TryParse("https://xyz.example.com:8443/api/token?_u=xxx", out assetTypes, out assets);
        Assert.Equal(typeof(Endpoint), assetTypes[0]);
        Assert.NotNull(assets[0]);
        Assert.Equal(typeof(Endpoint), assets[0].GetType());

        // TODO: more DNSRecord & Endpoint parsing tests
    }

    [Fact]
    public void ScopeChecker_Tests()
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
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tttesla.com")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla.com.net")));
        //Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla.com.test")));
        Assert.False(ScopeChecker.Singleton.IsInScope(new Domain("tesla2.com")));

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
        processor.ProcessAsync("xyz.tesla.com IN A 1.3.3.7").Wait();
        var host = context.Hosts.First(h => h.IP == "1.3.3.7");
        Assert.True(host.InScope);
    }

    [Fact]
    public void JobAssignment_Tests()
    {
        JobAssignmentService jobService = new();
        PwnctlDbContext context = new();

        jobService.Assign(new NetRange("172.16.17.0", 24));

        // blacklist test
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "nmap_basic"));

        jobService.Assign(new Endpoint("https", new Service(new Host("172.16.17.15"), 443), "/api/token"));
        // TaskDefinition.Filter fail test
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "ffuf_common"));

        // aggresivness test
        Assert.True(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "hakrawler"));
        Assert.False(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "sqlmap"));

        // Task.Command interpolation test
        var hakrawlerTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "hakrawler");
        Assert.Equal("hakrawler -plain -h 'User-Agent: Mozilla/5.0' https://172.16.17.15:443/api/token/", hakrawlerTask.Command);

        jobService.Assign(new Endpoint("https", new Service(new Host("172.16.17.15"), 443), "/"));
        
        // TaskDefinition.Filter pass test
        Assert.True(context.Tasks.Include(t => t.Definition).Any(t => t.Definition.ShortName == "ffuf_common"));

        // multiple interpolation test
        jobService.Assign(new Domain("sub.tesla.com"));
        var resolutionTask = context.Tasks.Include(t => t.Definition).First(t => t.Definition.ShortName == "domain_resolution");
        Assert.Equal("dig +short sub.tesla.com | awk '{print \"sub.tesla.com IN A \" $1}'| pwnctl process", resolutionTask.Command);

        // TODO: AllowActive = false test, csv black&whitelist test
    }

    [Fact]
    public void CachedPublicSuffixRepository_Tests()
    {
        var regDomain = CachedPublicSuffixRepository.Singleton.GetRegistrationDomain("xyz.example.com");
        var publicSuffix = CachedPublicSuffixRepository.Singleton.GetPublicSuffix("xyz.example.com");
        Assert.Equal("example.com", regDomain);
        Assert.Equal("com", publicSuffix.Suffix);

        regDomain = CachedPublicSuffixRepository.Singleton.GetRegistrationDomain("sub.example.azurewebsites.net");
        publicSuffix = CachedPublicSuffixRepository.Singleton.GetPublicSuffix("sub.example.azurewebsites.net");
        Assert.Equal("example.azurewebsites.net", regDomain);
        Assert.Equal("azurewebsites.net", publicSuffix.Suffix);
    }

    [Fact]
    public void AssetRepository_Tests()
    {
        AssetRepository repository = new();
        PwnctlDbContext context = new();

        var inScopeDomain = new Domain("tesla.com");
        var outOfScope = new Domain("www.outofscope.com");

        Assert.False(repository.CheckIfExists(inScopeDomain));
        repository.AddAsync(inScopeDomain).Wait();
        Assert.True(repository.CheckIfExists(inScopeDomain));
        inScopeDomain = context.Domains.First(d => d.Name == "tesla.com");
        Assert.True(inScopeDomain.InScope);
        repository.AddAsync(outOfScope).Wait();
        outOfScope = context.Domains.First(d => d.Name == "www.outofscope.com");
        Assert.False(outOfScope.InScope);

        var record1 = new DNSRecord(DNSRecord.RecordType.A, "hackerone.com", "1.3.3.7");
        var record2 = new DNSRecord(DNSRecord.RecordType.AAAA, "hackerone.com", "dead:beef::::");

        Assert.False(repository.CheckIfExists(record1));
        Assert.False(repository.CheckIfExists(record2));
        repository.AddAsync(record1).Wait();
        Assert.True(repository.CheckIfExists(record1));
        Assert.False(repository.CheckIfExists(record2));
        repository.AddAsync(record2).Wait();
        Assert.True(repository.CheckIfExists(record2));

        var netRange = new NetRange("10.1.101.0", 24);
        Assert.False(repository.CheckIfExists(netRange));
        repository.AddAsync(netRange).Wait();
        Assert.True(repository.CheckIfExists(netRange));

        var service = new Service(inScopeDomain, 443);
        Assert.False(repository.CheckIfExists(service));
        repository.AddAsync(service).Wait();
        Assert.True(repository.CheckIfExists(service));
    }

    [Fact]
    public void AssetProcessor_Tests()
    {
        AssetProcessor processor = new();
        PwnctlDbContext context = new();

        var res = processor.TryProccessAsync("tesla.com IN A 31.3.3.7").Result;
        Assert.True(res);

        var record = context.DNSRecords.First(r => r.Key == "tesla.com" && r.Value == "31.3.3.7");
        Assert.True(record.InScope);

        var domain = context.Domains.First(d => d.Name == "tesla.com");
        Assert.True(domain.InScope);

        var host = context.Hosts.Include(h => h.AARecords).First(host => host.IP == "31.3.3.7");
        Assert.True(host.InScope);
        host.AARecords.Add(record);
        Assert.NotNull(host.AARecords.First());
        Assert.True(host.AARecords.First().InScope);
        Assert.True(host.AARecords.First().Domain.InScope);
        var program = ScopeChecker.Singleton.GetApplicableProgram(host);
        Assert.NotNull(program);
    }

    [Fact]
    public void Tagging_Tests()
    {
        AssetProcessor processor = new();
        PwnctlDbContext context = new();

        BaseAsset[] assets = AssetParser.Parse("https://example.com [[ContentType:text/html][Status:200][Server:IIS]]", out Type[] assetTypes);

        var endpoint = assets.First(a => a.GetType() == typeof(Endpoint));
        Assert.NotNull(endpoint.Tags);

        var ctTag = endpoint.Tags.FirstOrDefault(t => t.Name == "ContentType");
        Assert.NotNull(ctTag);
        Assert.Equal("text/html", ctTag.Value);

        var stTag = endpoint.Tags.FirstOrDefault(t => t.Name == "Status");
        Assert.NotNull(stTag);
        Assert.Equal("200", stTag.Value);

        var srvTag = endpoint.Tags.FirstOrDefault(t => t.Name == "Server");
        Assert.NotNull(srvTag);
        Assert.Equal("IIS", srvTag.Value);

        processor.ProcessAsync("https://example.com [[ContentType:text/html][Status:200][Server:IIS]]").Wait();
        //Assert.True(res);

        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Uri == "https://example.com:443/").First();
        ctTag = endpoint.Tags.FirstOrDefault(t => t.Name == "ContentType");
        Assert.NotNull(ctTag);
        Assert.Equal("text/html", ctTag.Value);

        stTag = endpoint.Tags.FirstOrDefault(t => t.Name == "Status");
        Assert.NotNull(stTag);
        Assert.Equal("200", stTag.Value);

        srvTag = endpoint.Tags.FirstOrDefault(t => t.Name == "Server");
        Assert.NotNull(srvTag);
        Assert.Equal("IIS", srvTag.Value);

        // process same asset twice and make sure tasks are only assigned once
        processor.ProcessAsync("https://iis.tesla.com [[ContentType:text/html][Status:200][Server:IIS]]").Wait();
        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Uri == "https://iis.tesla.com:443/").First();
        var tasks = context.Tasks.Include(t => t.Definition).Where(t => t.EndpointId == endpoint.Id).ToList();
        Assert.True(!tasks.GroupBy(t => t.DefinitionId).Any(g => g.Count() > 1));

        // test Tag filter
        Assert.Contains(tasks, t => t.Definition.ShortName == "shortname_scanner");
        processor.ProcessAsync("https://apache.tesla.com [[ContentType:text/html][Status:200][Server:apache]]").Wait();
        endpoint = context.Endpoints.Include(e => e.Tags).Where(ep => ep.Uri == "https://apache.tesla.com:443/").First();
        tasks = context.Tasks.Include(t => t.Definition).Where(t => t.EndpointId == endpoint.Id).ToList();
        Assert.DoesNotContain(tasks, t => t.Definition.ShortName == "shortname_scanner");
    }

    // [Fact]
    // public void IP_Host_Url_Normalization_Tests()
    // {
        // TODO: IP_Host_Url_Normalization_Tests
    // }
}