using pwnctl.app.Tasks.Entities;
using pwnctl.app.Common.Interfaces;
using pwnctl.infra;
using pwnctl.infra.Aws;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using System.Diagnostics;
using Amazon.SQS.Model;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.svc
{
    public sealed class TaskConsumerService : BackgroundService
    {
        private readonly SQSTaskQueueService _queueService = new();
        private readonly PwnctlDbContext _context = new();
        private List<Message> _unprocessedMessages = new List<Message>();
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        
        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
            => _hostApplicationLifetime = hostApplicationLifetime;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(TaskConsumerService)} started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var messages = await _queueService.ReceiveAsync(stoppingToken);
                    if (messages == null || !messages.Any())
                    {
                        PwnContext.Logger.Information($"queue is empty");
                        break;
                    }

                    _unprocessedMessages = new(messages);

                    var timer = new System.Timers.Timer(1000 * 60 * (AwsConstants.QueueVisibilityTimeoutInSec - 1));
                    timer.Elapsed += async (sender, e) => await ChallengeMessageVisibility(stoppingToken);
                    timer.AutoReset = false;
                    timer.Start();

                    foreach (var message in messages)
                    {
                        var queuedTask = Serializer.Instance.Deserialize<TaskEntity>(message.Body);

                        var taskRecord = await _context
                                            .JoinedTaskRecordQueryable()
                                            .FirstOrDefaultAsync(r => r.Id == queuedTask.TaskId);
                                            
                        if (taskRecord == null)
                        {
                            PwnContext.Logger.Error($"Task: {queuedTask.TaskId}:'{queuedTask.Command}' not found");
                            await _queueService.DequeueAsync(message, stoppingToken);
                            continue;
                        }

                        taskRecord.Started();

                        Process process = await ExecuteCommandAsync(queuedTask.Command, taskRecord.Definition, stoppingToken);

                        // TODO: log stdout&stderr if ExitCode != 0
                        taskRecord.Finished(process.ExitCode);
                        
                        _context.Update(taskRecord);
                        await _context.SaveChangesAsync();

                        await _queueService.DequeueAsync(message, CancellationToken.None);

                        _unprocessedMessages.Remove(message);
                    }
                }
            }
            catch (Exception ex)
            {
                PwnContext.Logger.Error(ex.ToRecursiveExInfo());
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task<Process> ExecuteCommandAsync(string command, TaskDefinition definition, CancellationToken stoppingToken)
        {
            PwnContext.Logger.Debug(command);
            
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var process = Process.Start(psi);
            {
                if (process == null)
                    throw new Exception("bash process failed to start");

                using (StreamWriter sr = process.StandardInput)
                {
                    await sr.WriteLineAsync(@$"{command} | while read assetLine;
do 
    if [[ ${{assetLine::1}} == '{{' ]]; 
    then 
        echo $assetLine | jq -c '.tags += {{""FoundBy"": ""{definition.ShortName}""}}';
    else 
        echo '{{""asset"":""'$assetLine'"", ""tags"":{{""FoundBy"":""{definition.ShortName}""}}}}'; 
    fi; 
done".Replace("\r\n", "").Replace("\n", ""));
                    sr.Flush();
                    sr.Close();
                }
            }

            await process.WaitForExitAsync(stoppingToken);

            var queueService = new SQSTaskQueueService();
            var processor = AssetProcessorFactory.Create(queueService);
            var line = await process.StandardOutput.ReadLineAsync();
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                try
                {
                    await processor.ProcessAsync(line);
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                }
            }

            PwnContext.Logger.Debug($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            return process;
        }

        public async Task ChallengeMessageVisibility(CancellationToken stoppingToken)
        {
            await _queueService.ChangeBatchVisibility(_unprocessedMessages, stoppingToken);
        }
    }
}