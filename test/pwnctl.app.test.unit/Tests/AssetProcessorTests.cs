namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
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
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;

using Xunit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[Collection("UnitTests")]
public sealed class AssetProcessorTests
{
    public AssetProcessorTests()
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

        await proc.ProcessAsync("9.8.7.6:8443\tvhost.tesla.com", EntityFactory.TaskRecord.Id);
        record = context.AssetRecords.First(r => r.VirtualHost.SocketAddress == "tcp://9.8.7.6:8443");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.DomainName.Name == "vhost.tesla.com");
        Assert.True(record.InScope);

        record = context.AssetRecords.First(r => r.NetworkSocket.Address == "tcp://9.8.7.6:8443");
        Assert.False(record.InScope);

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
    public async Task TaskRecord_FoundByTask()
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
