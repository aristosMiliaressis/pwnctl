using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Logging;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Commands;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Queues;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Queueing.DTO;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private readonly AssetProcessor _processor = AssetProcessorFactory.Create();

        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _hostApplicationLifetime.ApplicationStopping.Register(() => 
            {
                PwnInfraContext.Logger.Information(LogSinks.File | LogSinks.Notification, $"{nameof(TaskConsumerService)} stoped.");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.Logger.Information(LogSinks.File | LogSinks.Notification, $"{nameof(TaskConsumerService)} started.");

            PwnctlDbContext context = new();

            while (!stoppingToken.IsCancellationRequested)
            {
                var taskDTOs = await _queueService.ReceiveAsync(stoppingToken);
                if (taskDTOs == null || !taskDTOs.Any())
                {
                    PwnInfraContext.Logger.Information("queue is empty");
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

                    var commandCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    commandCancellationToken.CancelAfter(TimeSpan.FromHours(1));

                    var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, taskEntry.WrappedCommand, commandCancellationToken.Token);

                    foreach (var line in process.StandardOutput.ReadToEnd().Split("\n").Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        try
                        {
                            await _processor.ProcessAsync(line);
                        }
                        catch (Exception ex)
                        {
                            PwnInfraContext.Logger.Exception(ex);
                        }

                        var pendingTasks = await context.JoinedTaskRecordQueryable()
                                    .Where(r => r.State == TaskState.PENDING)
                                    .ToListAsync(stoppingToken);

                        pendingTasks.ForEach(async task => 
                        {
                            task.Queued();
                            await _queueService.EnqueueAsync(new QueueTaskDTO(task));
                        });
                        
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    taskEntry.Finished(process.ExitCode);
                }
                catch (TaskCanceledException ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                    continue;
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
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
