using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.app.Assets;
using pwnctl.app.Tasks.Interfaces;
using Microsoft.EntityFrameworkCore;
using pwnctl.infra.Notifications;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Configuration;
using pwnctl.app.Tasks.Enums;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private readonly NotificationSender _notificationSender = new DiscordNotificationSender();

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _hostApplicationLifetime.ApplicationStopping.Register(() => 
            {
                PwnContext.Logger.Information($"{nameof(TaskConsumerService)} stoped.");
                _notificationSender.Send($"pwnctl service stoped on {EnvironmentVariables.HOSTNAME}", "status");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(TaskConsumerService)} started.");
            _notificationSender.Send($"pwnctl service started on {EnvironmentVariables.HOSTNAME}", "status");

            PwnctlDbContext context = new();

            while (!stoppingToken.IsCancellationRequested)
            {
                var taskDTOs = await _queueService.ReceiveAsync(stoppingToken);
                if (taskDTOs == null || !taskDTOs.Any())
                {
                    PwnContext.Logger.Information("queue is empty");
                    break;
                }

                var timer = new System.Timers.Timer(1000 * (AwsConstants.QueueVisibilityTimeoutInSec - 10));
                timer.Elapsed += async (_, _) => await _queueService.ChangeBatchVisibility(taskDTOs, stoppingToken);
                timer.Start();

                var taskDTO = taskDTOs[0];

                var taskEntry = await context
                                    .JoinedTaskRecordQueryable()
                                    .FirstOrDefaultAsync(r => r.Id == taskDTO.TaskId);

                try
                {
                    taskEntry.Started();

                    var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, taskEntry.WrappedCommand, stoppingToken);

                    foreach (var line in process.StandardOutput.ReadToEnd().Split("\n").Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        try
                        {
                            await _processor.ProcessAsync(line);
                        }
                        catch (Exception ex)
                        {
                            PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                        }

                        var pendingTasks = await context.JoinedTaskRecordQueryable()
                                    .Where(r => r.State == TaskState.PENDING)
                                    .ToListAsync(stoppingToken);

                        pendingTasks.ForEach(async task => 
                        {
                            task.Queued();
                            await _queueService.EnqueueAsync(task.ToDTO());
                        });
                        
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    taskEntry.Finished(process.ExitCode);
                }
                catch (TaskCanceledException ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    continue;
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    await _queueService.DequeueAsync(taskDTO);
                    continue;
                }

                await context.SaveChangesAsync();

                await _queueService.DequeueAsync(taskDTO);
            }
            
            _hostApplicationLifetime.StopApplication();
        }
    }
}
