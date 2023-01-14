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
using pwnctl.infra.Queueing;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Tasks.Entities;

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
                PwnInfraContext.Logger.Information(LogSinks.File|LogSinks.Notification, $"{nameof(TaskConsumerService)} stoped.");
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

                var taskDTO = taskDTOs[0];
                var taskEntry = await context
                                    .JoinedTaskRecordQueryable()
                                    .FirstOrDefaultAsync(r => r.Id == taskDTO.TaskId);

                try
                {
                    var commandCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    commandCancellationToken.CancelAfter(TimeSpan.FromSeconds(AwsConstants.QueueVisibilityTimeoutInSec - 60));

                    var outputLines = await ExecuteTaskAsync(taskEntry, commandCancellationToken.Token);
                    foreach (var line in outputLines.Where(a => !string.IsNullOrEmpty(a)))
                    {
                        await _processor.TryProcessAsync(line);

                        var pendingTasks = await context.JoinedTaskRecordQueryable()
                                    .Where(r => r.State == TaskState.PENDING)
                                    .ToListAsync();

                        pendingTasks.ForEach(async task =>
                        {
                            await _queueService.EnqueueAsync(new QueueTaskDTO(task));
                            task.Queued();
                        });
                    }

                    await context.SaveChangesAsync();
                }
                catch (TaskCanceledException ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                    continue;
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                }

                await _queueService.DequeueAsync(taskDTO);
            }

            _hostApplicationLifetime.StopApplication();
        }

        public static async Task<string[]> ExecuteTaskAsync(TaskEntry task, CancellationToken token = default)
        {
            string wrappedCommand = @$"{task.Command} | while read assetLine;
do
  if [[ ${{assetLine::1}} == '{{' ]];
  then
    echo $assetLine | jq -c '.tags += {{""FoundBy"": ""{task.Definition.ShortName}""}}';
  else
    echo '{{""asset"":""'$assetLine'"", ""tags"":{{""FoundBy"":""{task.Definition.ShortName}""}}}}';
  fi;
done".Replace("\r\n", "").Replace("\n", "");

            task.Started();
            var process = await CommandExecutor.ExecuteAsync("/bin/bash", null, wrappedCommand, token);
            var output = await process.StandardOutput.ReadToEndAsync();
            task.Finished(process.ExitCode);

            return output.Split("\n");
        }
    }
}
