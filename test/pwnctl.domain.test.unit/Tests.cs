namespace pwnctl.domain.test.unit;

using System;
using pwnctl.domain.Entities;
using pwnctl.domain.Interfaces;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Commands;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using Xunit;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
    }

    [Fact]
    public void PublicSuffixRepository_Tests()
    {
        var exampleDomain = new DomainName("xyz.example.com");

        Assert.Equal("example.com", exampleDomain.GetRegistrationDomain());
        Assert.Equal("com", PublicSuffixRepository.Instance.GetSuffix(exampleDomain.Name).Value.Value);

        var exampleSubDomain = new DomainName("sub.example.azurewebsites.net");

        Assert.Equal("example.azurewebsites.net", exampleSubDomain.GetRegistrationDomain());
        Assert.Equal("azurewebsites.net", PublicSuffixRepository.Instance.GetSuffix(exampleSubDomain.Name).Value.Value);
    }

    [Fact]
    public void DomainEntity_Tests()
    {
        Assert.Equal("example", new DomainName("example.com.").Word);
        Assert.Equal("example2", new DomainName("example2.azurewebsites.net").Word);

        Assert.Equal("example2.azurewebsites.net", new DomainName("sub.example2.azurewebsites.net").GetRegistrationDomain());

        Assert.True(NetworkRange.RoutesTo("1.3.3.7", "1.3.3.0/24"));
        Assert.False(NetworkRange.RoutesTo("1.3.3.7", "1.3.4.0/24"));

        var hosts = DomainNameRecord.ParseSPFString("tesla.com IN TXT \"v=spf1 ip4:2.2.2.2 ipv4: 3.3.3.3 ipv6:FD00:DEAD:BEEF:64:34::2 include: spf.protection.outlook.com include:servers.mcsv.net -all\"");

        Assert.Contains(hosts, h => h.IP == "2.2.2.2");
        Assert.Contains(hosts, h => h.IP == "3.3.3.3");
        Assert.Contains(hosts, h => h.IP == "fd00:dead:beef:64:34::2");
    }
}
