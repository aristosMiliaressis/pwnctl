using pwnwrk.domain.Tasks.Entities;
using pwnwrk.infra;
using pwnwrk.infra.Aws;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using System.Diagnostics;
using Amazon.SQS.Model;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.svc
{
    public sealed class JobConsumerService : BackgroundService
    {
        private readonly SQSJobQueueService _queueService = new();
        private readonly PwnctlDbContext _context = new();
        private List<Message> _unprocessedMessages = new List<Message>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(JobConsumerService)} started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var messages = await _queueService.ReceiveAsync(stoppingToken);
                    if (messages == null || !messages.Any())
                    {
                        PwnContext.Logger.Information($"queue is empty");
                        Thread.Sleep(1000*60*5);
                        Environment.Exit(0);
                    }

                    _unprocessedMessages = new(messages);

                    var timer = new System.Timers.Timer(1000 * 60 * (AwsConstants.QueueVisibilityTimeoutInSec - 1));
                    timer.Elapsed += async (sender, e) => await ChallengeMessageVisibility(stoppingToken);
                    timer.AutoReset = false;
                    timer.Start();

                    foreach (var message in messages)
                    {
                        var queuedTask = PwnContext.Serializer.Deserialize<TaskEntity>(message.Body);

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

                        await _queueService.DequeueAsync(message, stoppingToken);

                        _unprocessedMessages.Remove(message);
                    }
                }
            }
            catch (Exception ex)
            {
                PwnContext.Logger.Error(ex.ToRecursiveExInfo());
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
done | pwnwrk".Replace("\r\n", "").Replace("\n", ""));
                    sr.Flush();
                    sr.Close();
                }
            }

            await process.WaitForExitAsync(stoppingToken);

            PwnContext.Logger.Debug($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            return process;
        }

        public async Task ChallengeMessageVisibility(CancellationToken stoppingToken)
        {
            await _queueService.ChangeBatchVisibility(_unprocessedMessages, stoppingToken);
        }
    }
}