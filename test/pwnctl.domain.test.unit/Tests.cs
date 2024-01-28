namespace pwnctl.domain.test.unit;

using System;
using pwnctl.domain.Entities;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Queueing;
using pwnctl.app.Queueing.Interfaces;
using Xunit;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        PwnInfraContextInitializer.Setup();
    }

    [Fact]
    public void DomainName_GetRegistrationDomain()
    {
        var domain1 = DomainName.TryParse("example.com").Value;
        var domain2 = DomainName.TryParse("deep.sub.example2.azurewebsites.net").Value;
        Assert.Equal("example.com", domain1.Name);
        Assert.Equal("example", domain1.Word);
        Assert.Equal(1, domain1.ZoneDepth);
        Assert.Equal("deep.sub.example2.azurewebsites.net", domain2.Name);
        Assert.Equal("example2", domain2.Word);
        Assert.Equal(3, domain2.ZoneDepth);

        Assert.Equal("example.com", domain1.GetRegistrationDomain());
        Assert.Equal("example2.azurewebsites.net", domain2.GetRegistrationDomain());      
    }

    [Fact]
    public void NetworkRange_RoutesTo()
    {
        Assert.True(NetworkRange.RoutesTo("1.3.3.7", "1.3.3.0/24"));
        Assert.False(NetworkRange.RoutesTo("1.3.3.7", "1.3.4.0/24"));
    }

    [Fact]
    public void DomainNameRecord_ParseSPFString()
    {
        var hosts = DomainNameRecord.ParseSPFString("tesla.com IN TXT \"v=spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net -all\"");

        Assert.Contains(hosts, h => h.IP == "2.2.2.2");
        Assert.Contains(hosts, h => h.IP == "3.3.3.3");
        Assert.Contains(hosts, h => h.IP == "fd00:dead:beef:64:34::2");
    }

    [Fact]
    public void DomainNameRecord_PTR_To_NetworkHost()
    {
        var record = DomainNameRecord.TryParse("12.34.56.78.in-addr.arpa IN PTR dns.google.").Value;
        Assert.NotNull(record.NetworkHost);
        Assert.Equal("78.56.34.12", record.NetworkHost.ToString());
    }

    [Fact]
    public void NetworkHost_IsPrivate()
    {
        var host1 = NetworkHost.TryParse("172.18.30.6").Value;
        Assert.True(host1.IsPrivate());

        var host2 = NetworkHost.TryParse("192.168.1.1").Value;
        Assert.True(host2.IsPrivate());

        var host3 = NetworkHost.TryParse("10.0.0.1").Value;
        Assert.True(host3.IsPrivate());

        var host4 = NetworkHost.TryParse("127.0.0.1").Value;
        Assert.True(host4.IsPrivate());

        var host5 = NetworkHost.TryParse("8.8.8.8").Value;
        Assert.False(host5.IsPrivate());
    }

    [Fact]
    public void HttpEndpoint_IsIpBased()
    {
        var endpoint1 = HttpEndpoint.TryParse("https://1.3.3.7/api").Value;
        Assert.True(endpoint1.IsIpBased);

        // TODO: fix this
        // var endpoint2 = HttpEndpoint.TryParse("https://[fd00:dead:beef:64:34::2]/api").Value;
        // Assert.True(endpoint2.IsIpBased);

        var endpoint3 = HttpEndpoint.TryParse("https://example.com/api").Value;
        Assert.False(endpoint3.IsIpBased);
    }

    [Fact]
    public void HttpEndpoint_BaseEndpoint()
    {
        var endpoint1 = HttpEndpoint.TryParse("https://1.3.3.7/api/v2/token").Value;
        Assert.NotNull(endpoint1.BaseEndpoint);
        Assert.Null(endpoint1.BaseEndpoint.BaseEndpoint);
        Assert.Equal("https://1.3.3.7/", endpoint1.BaseEndpoint.ToString());
    }
}
