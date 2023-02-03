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
                PwnInfraContext.Logger.Information(LogSinks.Console|LogSinks.Notification, $"{nameof(TaskConsumerService)} stoped.");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.Logger.Information(LogSinks.Console | LogSinks.Notification, $"{nameof(TaskConsumerService)} started.");

            while (!stoppingToken.IsCancellationRequested)
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

                // create a linked token that cancels the task when the timeout passes or 
                // when a SIGTERM is received due to an ECS scale in event
                var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromHours(2));

                // Change the message visibility if the visibility window is exheeded
                // this allows us to keep a smaller vid\sibility window without effecting 
                // the max task timeout.
                var timer = new System.Timers.Timer(1000 * (AwsConstants.QueueVisibilityTimeoutInSec - 60));
                timer.Elapsed += async (_, _) =>
                    await _queueService.ChangeMessageVisibilityAsync(taskDTO, AwsConstants.QueueVisibilityTimeoutInSec);
                timer.Start();

                try
                {
                    await ExecuteTaskAsync(taskEntry, cts.Token);
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);

                    // return the task to the queue, if this occures to many times it will be put in the dead letter queue
                    await _queueService.ChangeMessageVisibilityAsync(taskDTO, 0);
                    continue;
                }
                finally
                {
                    timer.Dispose();
                }

                await _queueService.DequeueAsync(taskDTO);
            }

            _hostApplicationLifetime.StopApplication();
        }

        public async Task ExecuteTaskAsync(TaskEntry task, CancellationToken token = default)
        {
            task.Started();
            await _context.SaveChangesAsync(token);

            var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, task.Command, token);
            var output = await process.StandardOutput.ReadToEndAsync();

            foreach (var line in output.Split("\n").Where(a => !string.IsNullOrEmpty(a)))
            {
                await _processor.TryProcessAsync(line, task);

                var pendingTasks = await _context.JoinedTaskRecordQueryable()
                            .Where(r => r.State == TaskState.PENDING)
                            .ToListAsync();

                foreach (var t in pendingTasks)
                {
                    t.Queued();
                    await _queueService.EnqueueAsync(new QueuedTaskDTO(t));
                    await _context.SaveChangesAsync();
                }
            }

            string stderr = await process.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(stderr))
                PwnInfraContext.Logger.Error(stderr);

            task.Finished(process.ExitCode);
            await _context.SaveChangesAsync();
        }
    }
}
