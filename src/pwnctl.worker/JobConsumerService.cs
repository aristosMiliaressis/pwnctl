using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
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
                var messages = await _queueService.ReceiveAsync(stoppingToken);
                if (messages == null || !messages.Any())
                {
                    Thread.Sleep(60000);
                    continue;
                }

                _unprocessedMessages = messages;

                var timer = new System.Timers.Timer(1000 * 60 * (pwnctl.infra.Configuration.ConfigurationManager.Config.JobQueue.VisibilityTimeout - 1));
                timer.Elapsed += async (sender, e) => await ChallengeMessageVisibility();
                timer.AutoReset = false;
                timer.Start();
                    
                foreach (var message in messages)
                {
                    var task = JsonSerializer.Deserialize<TaskAssigned>(message.Body);

                    bool succeded = await ExecuteCommandAsync(task.Command, stoppingToken);
                    //if (succeded)
                    await _queueService.DequeueAsync(message, stoppingToken);
                    //else
                    // TODO: move to dead letter queue

                    _unprocessedMessages.Remove(message);
                }
            }            
        }

        private async Task<bool> ExecuteCommandAsync(string command, CancellationToken stoppingToken)
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
                    return false;

                using (StreamWriter sr = process.StandardInput)
                {
                    await sr.WriteLineAsync(command);
                    sr.Flush();
                    sr.Close();
                }
            }

            await process.WaitForExitAsync(stoppingToken);

            // TODO: record metadata
            Logger.Instance.Info($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            //process.ExitCode
            //process.ExitTime
            //var output = await process.StandardOutput.ReadToEndAsync();
            //var errors = await process.StandardError.ReadToEndAsync();

            return true;
        }

        public async Task ChallengeMessageVisibility()
        {
            await _queueService.ChangeBatchVisibility(_unprocessedMessages, _stoppingToken);
        }
    }
}