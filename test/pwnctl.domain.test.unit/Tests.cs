namespace pwnctl.domain.test.unit;

using System;
using pwnctl.domain.Entities;
using pwnctl.domain.Interfaces;
using pwnctl.infra.DependencyInjection;

using Xunit;

public sealed class Tests
{
    public Tests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_TEST_RUN", "true");
        Environment.SetEnvironmentVariable("PWNCTL_USE_SQLITE", "true");
        Environment.SetEnvironmentVariable("PWNCTL_INSTALL_PATH", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__FilePath", ".");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Debug");

        PwnInfraContextInitializer.SetupAsync().Wait();
    }

    [Fact]
    public void PublicSuffixRepository_Tests()
    {
        var exampleDomain = new DomainName("xyz.example.com");

        Assert.Equal("example.com", exampleDomain.GetRegistrationDomain());
        Assert.Equal("com", PublicSuffixRepository.Instance.GetSuffix(exampleDomain.Name).Value);

        var exampleSubDomain = new DomainName("sub.example.azurewebsites.net");

        Assert.Equal("example.azurewebsites.net", exampleSubDomain.GetRegistrationDomain());
        Assert.Equal("azurewebsites.net", PublicSuffixRepository.Instance.GetSuffix(exampleSubDomain.Name).Value);
    }

    [Fact]
    public void CloudServiceRepository_Tests()
    {
        var ep1 = new HttpEndpoint("https", new NetworkSocket(new DomainName("example.com"), 443), "/");
        var ep2 = new HttpEndpoint("https", new NetworkSocket(new DomainName("example.s3.amazonaws.com"), 443), "/");

        Assert.False(CloudServiceRepository.Instance.IsCloudService(ep1));
        Assert.True(CloudServiceRepository.Instance.IsCloudService(ep2));
    }

    [Fact]
    public void DomainEntity_Tests()
    {
        // TODO: Implement
        //Domain.GetRegistrationDomain/Word
        //DomainNameRecord.ParseSPFString(spfv1&spfv2)
        //HttpEndpoint.Path/Filename/Extension
        //NetworkRagne.RouteTo(ipv4|ipv6)
    }
}
