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
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
    }

    [Fact]
    public void DomainEntity_Tests()
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

        Assert.True(NetworkRange.RoutesTo("1.3.3.7", "1.3.3.0/24"));
        Assert.False(NetworkRange.RoutesTo("1.3.3.7", "1.3.4.0/24"));

        var hosts = DomainNameRecord.ParseSPFString("tesla.com IN TXT \"v=spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net -all\"");

        Assert.Contains(hosts, h => h.IP == "2.2.2.2");
        Assert.Contains(hosts, h => h.IP == "3.3.3.3");
        Assert.Contains(hosts, h => h.IP == "fd00:dead:beef:64:34::2");
    }
}
