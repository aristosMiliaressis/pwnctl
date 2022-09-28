using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using pwnctl.infra.Persistence;
using System.Diagnostics;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Options;

namespace pwnctl.worker
{
    public class JobConsumerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppConfig> _config;
        private readonly SQSJobQueueService _queueService = new();
        private readonly PwnctlDbContext _context = new();
        private CancellationToken _stoppingToken;
        private List<Amazon.SQS.Model.Message> _unprocessedMessages = new List<Amazon.SQS.Model.Message>();

        public JobConsumerService(ILogger<JobConsumerService> logger, IOptions<AppConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Instance.Info($"{nameof(JobConsumerService)} started.");
            _stoppingToken = stoppingToken;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await _queueService.ReceiveAsync(stoppingToken);
                    if (messages == null || !messages.Any())
                    {
                        Thread.Sleep(60000);
                        continue;
                    }

                    _unprocessedMessages = new(messages);

                    var timer = new System.Timers.Timer(1000 * 60 * (pwnctl.infra.Configuration.ConfigurationManager.Config.JobQueue.VisibilityTimeout - 1));
                    timer.Elapsed += async (sender, e) => await ChallengeMessageVisibility();
                    timer.AutoReset = false;
                    timer.Start();

                    foreach (var message in messages)
                    {                      
                        var queuedTask = JsonSerializer.Deserialize<TaskAssigned>(message.Body);

                        var task = await _context.Tasks.FindAsync(queuedTask.TaskId);
                        if (task == null)
                        {
                            // normaly this should never happen, log and continue.
                            Logger.Instance.Info($"Task: {queuedTask.TaskId}:'{queuedTask.Command}' not found");
                            await _queueService.DequeueAsync(message, stoppingToken);
                            continue;
                        }

                        task.StartedAt = DateTime.UtcNow;

                        Process process = await ExecuteCommandAsync(queuedTask.Command, stoppingToken);
                        if (process == null || process.HasExited)
                        {
                            // 
                        }

                        task.FinishedAt = DateTime.UtcNow;
                        task.ReturnCode = process.ExitCode;
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
                    Logger.Instance.Info(ex.ToRecursiveExInfo());
                }
            }            
        }

        private async Task<Process> ExecuteCommandAsync(string command, CancellationToken stoppingToken)
        {
            Logger.Instance.Info(command);
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            {
                if (process == null)
                    return null;

                using (StreamWriter sr = process.StandardInput)
                {
                    await sr.WriteLineAsync(command);
                    sr.Flush();
                    sr.Close();
                }
            }

            await process.WaitForExitAsync(stoppingToken);
            
            //Logger.Instance.Info($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            return process;
        }

        public async Task ChallengeMessageVisibility()
        {
            await _queueService.ChangeBatchVisibility(_unprocessedMessages, _stoppingToken);
        }
    }
}