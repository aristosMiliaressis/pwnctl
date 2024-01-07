namespace pwnctl.exec.test.integration;

using pwnctl.app.Assets;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra.Queueing;
using pwnctl.infra.Persistence;
using pwnctl.core.test.integration.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public sealed class TaskExecutionTests : IntegrationTestBase
{
    [Fact]
    public async Task Execute_LongLiving_Tasks()
    {
        var context = new PwnctlDbContext();
        var taskQueue = new SQSTaskQueueService();
        var processor = new AssetProcessor();

        var op = EntityFactory.EnsureMonitorOperationCreated();
        var asset = context.AssetRecords.First(r => r.DomainNameId != null);

        var subEnum = context.TaskDefinitions.First(d => d.Name == ShortName.Create("sub_enum"));
        var task = new TaskRecord(op, subEnum, asset);
        context.Add(task);
        context.SaveChanges();
        var taskDTO = new LongLivedTaskDTO(task);
        taskDTO.Command = Guid.NewGuid().ToString();
        await taskQueue.EnqueueAsync(taskDTO);

        Assert.Equal(TaskState.QUEUED, task.State);
        Thread.Sleep(15000);

        context = new PwnctlDbContext();
        task = context.TaskRecords.First(t => t.Id == task.Id);
        Assert.Equal(TaskState.FINISHED, task.State);
        Assert.Equal(1, task.RunCount);
    }

    [Fact]
    public async Task Execute_ShortLived_Tasks()
    {
        var context = new PwnctlDbContext();
        var taskQueue = new SQSTaskQueueService();
        var processor = new AssetProcessor();

        var op = EntityFactory.EnsureMonitorOperationCreated();
        var asset = context.AssetRecords.First(r => r.DomainNameId != null);

        var domain_resolution = context.TaskDefinitions.First(d => d.Name == ShortName.Create("domain_resolution"));
        var task = new TaskRecord(op, domain_resolution, asset);
        context.Add(task);
        context.SaveChanges();
        var taskDTO = new ShortLivedTaskDTO(task);
        taskDTO.Command = Guid.NewGuid().ToString();
        await taskQueue.EnqueueAsync(taskDTO);

        Assert.Equal(TaskState.QUEUED, task.State);
        Thread.Sleep(15000);

        context = new PwnctlDbContext();
        task = context.TaskRecords.First(t => t.Id == task.Id);
        Assert.Equal(TaskState.FINISHED, task.State);
        Assert.Equal(1, task.RunCount);
    }

    [Fact]
    public async Task Process_Output_Batch() 
    {
        var context = new PwnctlDbContext();
        var taskQueue = new SQSTaskQueueService();
        var processor = new AssetProcessor();

        var op = EntityFactory.EnsureCrawlOperationCreated();

        var asset = context.AssetRecords.First(r => r.TextNotation == "starlink.com");

        var subEnum = context.TaskDefinitions.First(d => d.Name == ShortName.Create("sub_enum"));
        var task = new TaskRecord(op, subEnum, asset);
        task.Started();
        task.Finished(0, null);
        context.Add(task);
        context.SaveChanges();

        // populate output queue
        var lines = new List<string> {
            "example.com",
            "starlink.com",
            "sub2.starlink.com",
            "1.2.3.4",
            "sub2.starlink.com IN A 1.2.3.4"
        };

        var batches = OutputBatchDTO.FromLines(lines, task.Id);
        await taskQueue.EnqueueAsync(batches[0]);

        Thread.Sleep(10000);

        context = new PwnctlDbContext();
        var host = context.NetworkHosts.First(h => h.IP == "1.2.3.4");
        var tasks = context.TaskRecords.Where(t => t.RecordId == host.Id).ToList();
        Assert.Equal(5, tasks.Count());
        Assert.All(tasks, t => Assert.Equal(TaskState.FINISHED, t.State));
    }

    // Execute Task with State Started,Running,Finished,Failed
    // Execute Task not found
    // Timeout
    // Change MessageVisibility
    // Exit on error
}