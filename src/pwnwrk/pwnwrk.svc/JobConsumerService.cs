using pwnwrk.infra;
using pwnwrk.infra.Aws;
using pwnwrk.infra.Configuration;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Persistence;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace pwnwrk.svc
{
    public sealed class JobConsumerService : BackgroundService
    {
        private readonly IOptions<AppConfig> _config;
        private readonly SQSJobQueueService _queueService = new();
        private readonly PwnctlDbContext _context = new();
        private CancellationToken _stoppingToken;
        private List<Amazon.SQS.Model.Message> _unprocessedMessages = new List<Amazon.SQS.Model.Message>();

        public JobConsumerService(IOptions<AppConfig> config)
        {
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(JobConsumerService)} started.");
            _stoppingToken = stoppingToken;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await _queueService.ReceiveAsync(stoppingToken);
                    if (messages == null || !messages.Any())
                    {
                        PwnContext.Logger.Information($"queue is empty");
                        Thread.Sleep(60000);
                        continue;
                    }

                    _unprocessedMessages = new(messages);

                    var timer = new System.Timers.Timer(1000 * 60 * (AwsConstants.QueueVisibilityTimeoutInSec - 1));
                    timer.Elapsed += async (sender, e) => await ChallengeMessageVisibility();
                    timer.AutoReset = false;
                    timer.Start();

                    foreach (var message in messages)
                    {                      
                        var queuedTask = PwnContext.Serializer.Deserialize<TaskAssigned>(message.Body);

                        var task = await _context.TaskRecords.FindAsync(queuedTask.TaskId);
                        if (task == null)
                        {
                            // normaly this should never happen, log and continue.
                            PwnContext.Logger.Error($"Task: {queuedTask.TaskId}:'{queuedTask.Command}' not found");
                            await _queueService.DequeueAsync(message, stoppingToken);
                            continue;
                        }

                        task.Started();

                        Process process = await ExecuteCommandAsync(queuedTask.Command, stoppingToken);

                        task.Finished(process.ExitCode);
                        
                        _context.Update(task);
                        await _context.SaveChangesAsync();

                        //if (process.ExitCode == 0) ??
                        await _queueService.DequeueAsync(message, stoppingToken);
                        //else
                        // move to dead letter queue

                        _unprocessedMessages.Remove(message);
                    }
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                }
            }            
        }

        private async Task<Process> ExecuteCommandAsync(string command, CancellationToken stoppingToken)
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
                    await sr.WriteLineAsync(command);
                    sr.Flush();
                    sr.Close();
                }
            }

            await process.WaitForExitAsync(stoppingToken);

            PwnContext.Logger.Debug($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            return process;
        }

        public async Task ChallengeMessageVisibility()
        {
            await _queueService.ChangeBatchVisibility(_unprocessedMessages, _stoppingToken);
        }
    }
}