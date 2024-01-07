namespace pwnctl.app.test.unit;

using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
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
using pwnctl.infra.Scheduling;
using pwnctl.infra.Queueing;
using pwnctl.infra.Notifications;

using Xunit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using pwnctl.kernel;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations;
using pwnctl.app.Operations.Enums;
using System.Reflection;

[Collection("Tests")]
public sealed class OperationHandlingTests
{
    public OperationHandlingTests()
    {
        Environment.SetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS", "true");
        Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");

        PwnInfraContextInitializer.Setup();
        DatabaseInitializer.InitializeAsync(Assembly.GetExecutingAssembly(), null).Wait();
        PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
        PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();
    }

    [Fact]
    public async Task OperationManager_Tests()
    {
        PwnctlDbContext context = new();
        AssetProcessor proc = new();
        AssetDbRepository assetRepo = new();
        OperationManager opManager = new(new OperationDbRepository(), new TaskDbRepository(), new EventBridgeClient());

        var op = new Operation("monitor_op", OperationType.Monitor, EntityFactory.Policy, EntityFactory.ScopeAggregate);

        var domain = DomainName.TryParse("deep.sub.tesla.com").Value;
        var record = new AssetRecord(domain);
        var domain2 = DomainName.TryParse("sub.tesla.com").Value;
        var record2 = new AssetRecord(domain2);
        var domain3 = DomainName.TryParse("tesla.com").Value;
        var record3 = new AssetRecord(domain3);
        record.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        record2.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        record3.SetScopeId(EntityFactory.ScopeAggregate.Definitions.First().DefinitionId);
        context.Entry(op.Scope).State = EntityState.Unchanged;
        context.Entry(op.Policy).State = EntityState.Unchanged;
        domain3.ParentDomain = null;
        context.Add(record3);
        domain2.ParentDomain = null;
        context.Add(record2);
        domain.ParentDomain = null;
        context.Add(record);
        context.Add(op);
        await context.SaveChangesAsync();

        // Schedule - no previous occurence - added
        await opManager.TryHandleAsync(op.Id);

        // PreCondition tests
        Assert.True(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("cloud_enum")));
        Assert.False(context.TaskRecords.Include(t => t.Definition).Any(t => t.Definition.Name == ShortName.Create("sub_enum")
                                                                          && t.Record.DomainName.Name == "deep.sub.tesla.com"));

        //  Schedule - previous occurance passed schedule - added
        SystemTime.SetDateTime(DateTime.UtcNow.AddDays(2));
        await opManager.TryHandleAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        //  Schedule - previous occurance not passed schedule - not added
        await opManager.TryHandleAsync(op.Id);

        Assert.Equal(2, context.TaskRecords.Include(t => t.Definition).Count(t => t.Definition.Name == ShortName.Create("sub_enum")));

        // PostCondition & NotificationTemplate tests
        record3.MergeTags(new Dictionary<string, string> { { "rcode", "NXDOMAIN" } }, true);
        await assetRepo.SaveAsync(record3);

        var task = context.TaskRecords
                            .Include(t => t.Definition)
                            .Include(t => t.Record)
                            .First(t => t.Definition.Name == ShortName.Create("domain_resolution")
                                   && t.Record.DomainName.Name == domain3.Name);

        await proc.TryProcessAsync($$$"""{"Asset":"{{{domain3.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        // check #1 rcode tag got updated
        var rcodeTag = context.Tags
                            .First(t => t.RecordId == record3.Id && t.Name == "rcode");
        Assert.Equal("SERVFAIL", rcodeTag.Value);

        context = new();
        // check #2 notification sent?
        var notification = context.Notifications
                            .Include(n => n.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(r => r.Record)
                                .ThenInclude(r => r.Tags)
                            .Include(n => n.Task)
                                .ThenInclude(t => t.Definition)
                            .First(n => n.TaskId == task.Id && n.Record.DomainName.Name == domain3.Name);
        Assert.Equal(task.Id, notification.TaskId.Value);
        Assert.Equal(record3.Id, notification.RecordId);

        // check #3 NotificationTemplate
        notification.Task = task;
        Assert.Equal("domain tesla.com changed rcode from NXDOMAIN to SERVFAIL", notification.GetText());

        // check #4 new asset notification
        await proc.TryProcessAsync($$$"""{"Asset":"new.{{{domain3.Name}}}","tags":{"rcode":"SERVFAIL"}}""", task.Id);

        var newDomain = context.DomainNames.First(d => d.Name == "new." + domain3.Name);

        notification = context.Notifications.FirstOrDefault(n => n.TaskId == task.Id && n.RecordId == newDomain.Id);
        Assert.NotNull(notification);

        // check #5 notificationRule.Template test
        var notificationRule = context.NotificationRules.First(n => n.Name == ShortName.Create("mdwfuzzer"));

        await proc.TryProcessAsync($$$"""{"Asset":"https://{{{domain3.Name}}}/","tags":{"mdwfuzzer_check":"uri-override-header"}}""", EntityFactory.TaskRecord.Id);
        var endpoint = context.HttpEndpoints.First(e => e.Url == "https://tesla.com/");
        notification = context.Notifications
                                .Include(n => n.Rule)
                                .Include(n => n.Record)
                                    .ThenInclude(n => n.Tags)
                                .First(n => n.RuleId == notificationRule.Id);
        Assert.Equal("https://tesla.com/ triggered mdwfuzzer check uri-override-header with word ", notification.GetText());

        // TODO: Add Scan Type Operation initialization test
    }

    [Fact]
    public async Task Initialize_Monitor_Operation()
    {
        // var context = new PwnctlDbContext();

        // // PwnInfraContextInitializer.Register<TaskQueueService, FakeTaskQueueService>();
        // // PwnInfraContextInitializer.Register<NotificationSender, StubNotificationSender>();

        // // var op = context.Operations.Where(o => o.Type == OperationType.Monitor).FirstOrDefault();
        // // if (op is null)
        // // {
        // //     op = EntityFactory.EnsureMonitorOperationCreated();
        // // }
        // var op = EntityFactory.EnsureMonitorOperationCreated();

        // Assert.Equal(OperationState.Pending, op?.State);
        // await _opManager.TryHandleAsync(op.Id);

        // context = new PwnctlDbContext();
        // op = context.Operations.Find(op.Id);

        // Assert.NotEqual(DateTime.MinValue, op?.InitiatedAt);
        // Assert.Equal(OperationState.Ongoing, op?.State);

        // // TODO: check that tasks were generated
        // // var task = context.TaskRecords.Include(t => t.Definition).First(t => t.Definition.Name == ShortName.Create("domain_resolution"));
        // // Assert.NotEqual(DateTime.MinValue, task?.QueuedAt);
        // // Assert.Equal(TaskState.QUEUED, task?.State);
    }

    [Fact]
    public async Task Initialize_Scan_Operation()
    {
        return;  // TODO: implement
    }

    [Fact]
    public async Task Terminate_Operation()
    {
        return;  // TODO: implement
    }

    [Fact]
    public async Task Transition_Operation_Phase()
    {
        return;  // TODO: implement
    }
}
