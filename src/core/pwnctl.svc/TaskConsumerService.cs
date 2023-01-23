using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Logging;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Commands;
using pwnctl.infra.Queueing;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private readonly PwnctlDbContext _context = new();

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _hostApplicationLifetime.ApplicationStopping.Register(() => 
            {
                PwnInfraContext.Logger.Information(LogSinks.File|LogSinks.Notification, $"{nameof(TaskConsumerService)} stoped.");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.Logger.Information(LogSinks.File | LogSinks.Notification, $"{nameof(TaskConsumerService)} started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var taskDTO = await _queueService.ReceiveAsync(stoppingToken);
                    if (taskDTO == null)
                    {
                        PwnInfraContext.Logger.Information("queue is empty");
                        break;
                    }

                    var taskEntry = await _context
                                        .JoinedTaskRecordQueryable()
                                        .FirstOrDefaultAsync(r => r.Id == taskDTO.TaskId);

                    try
                    {
                        var commandCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        commandCancellationToken.CancelAfter(TimeSpan.FromSeconds(AwsConstants.QueueVisibilityTimeoutInSec - 300));

                        await ExecuteTaskAsync(taskEntry, commandCancellationToken.Token);
                    }
                    catch (Exception ex)
                    {
                        PwnInfraContext.Logger.Exception(ex);
                    }
                    finally
                    {
                        await _queueService.DequeueAsync(taskDTO);
                    }
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                }
            }

            _hostApplicationLifetime.StopApplication();
        }

        public async Task ExecuteTaskAsync(TaskEntry task, CancellationToken token = default)
        {
            task.Started();
            await _context.SaveChangesAsync(token);

            var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, task.WrappedCommand, token);
            var output = await process.StandardOutput.ReadToEndAsync();

            foreach (var line in output.Split("\n").Where(a => !string.IsNullOrEmpty(a)))
            {
                await _processor.TryProcessAsync(line);

                var pendingTasks = await _context.JoinedTaskRecordQueryable()
                            .Where(r => r.State == TaskState.PENDING)
                            .ToListAsync();

                foreach (var t in pendingTasks)
                {
                    t.Queued();
                    await _queueService.EnqueueAsync(new QueueTaskDTO(t));
                    await _context.SaveChangesAsync();
                }
            }

            PwnInfraContext.Logger.Error(await process.StandardError.ReadToEndAsync());

            task.Finished(process.ExitCode);
            await _context.SaveChangesAsync();
        }
    }
}
