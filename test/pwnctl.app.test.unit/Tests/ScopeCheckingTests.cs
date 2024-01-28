namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Persistence;
using Xunit;
using System;
using System.Reflection;

[Collection("UnitTests")]
public sealed class ScopeCheckingTests
{
    public ScopeCheckingTests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
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

        // Virtual Host
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(VirtualHost.TryParse("172.16.17.15:8443\texample.com").Value));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(VirtualHost.TryParse("172.16.17.15:8443\texample.com").Value.Domain));
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(VirtualHost.TryParse("1.2.3.4:8443\ttesla.com").Value));
        Assert.Contains(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(VirtualHost.TryParse("1.2.3.4:8443\ttesla.com").Value.Domain));
        Assert.DoesNotContain(EntityFactory.ScopeAggregate.Definitions,
            scope => scope.Definition.Matches(VirtualHost.TryParse("1.2.3.4:8443\ttesla.com").Value.Socket));
    }
}
