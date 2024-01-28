namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
using pwnctl.app.Assets;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using Xunit;
using System;
using System.Linq;
using System.Reflection;

[Collection("UnitTests")]
public sealed class AssetParsingTests
{
    public AssetParsingTests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
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
        Assert.NotNull(((HttpEndpoint)result.Value).BaseEndpoint);

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
        
        // virtual host
        result = AssetParser.Parse("172.16.17.5:8443\ttesla.com");
        var vhost = result.Value as VirtualHost;
        Assert.NotNull(vhost.Socket);
        Assert.NotNull(vhost.Domain);
        Assert.Equal("tcp://172.16.17.5:8443", vhost.SocketAddress);
        Assert.Equal("tesla.com", vhost.Hostname);
        Assert.Equal("tcp://172.16.17.5:8443\ttesla.com", vhost.ToString());
    }
}
