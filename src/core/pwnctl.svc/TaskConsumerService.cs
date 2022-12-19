using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.app.Assets;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Models;
using Microsoft.EntityFrameworkCore;

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
            _hostApplicationLifetime.ApplicationStopping.Register(() => Console.WriteLine("ApplicationStopping called"));
            _hostApplicationLifetime.ApplicationStopped.Register(() => Console.WriteLine("ApplicationStopped called"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(TaskConsumerService)} started.");

            PwnctlDbContext context = new();

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _queueService.ReceiveAsync(stoppingToken);
                if (messages == null || !messages.Any())
                {
                    PwnContext.Logger.Information("queue is empty");
                    break;
                }

                var timer = new System.Timers.Timer(1000 * (AwsConstants.QueueVisibilityTimeoutInSec - 10));
                timer.Elapsed += async (_, _) => await _queueService.ChangeBatchVisibility(messages, stoppingToken);
                timer.Start();

                var message = messages[0];
                var queuedTask = message.Content as TaskDTO;

                var taskRecord = await context
                                    .JoinedTaskRecordQueryable()
                                    .FirstOrDefaultAsync(r => r.Id == queuedTask.TaskId);

                try
                {
                    taskRecord.Started();

                    var process = await CommandExecutor.ExecuteAsync("/bin/bash", taskRecord.WrappedCommand, stoppingToken);

                    foreach (var line in process.StandardOutput.ReadToEnd().Split("\n"))
                    {
                        try
                        {
                            await _processor.ProcessAsync(line);
                        }
                        catch (Exception ex)
                        {
                            PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                        }
                    }

                    taskRecord.Finished(process.ExitCode);
                }
                catch (TaskCanceledException ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    continue;
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    await _queueService.DequeueAsync(message);
                    continue;
                }

                await context.SaveChangesAsync();

                await _queueService.DequeueAsync(message);
            }
            
            _hostApplicationLifetime.StopApplication();
        }
    }
}
