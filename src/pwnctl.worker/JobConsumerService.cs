using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.infra.Logging;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace pwnctl.worker
{
    public class JobConsumerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppConfig> _config;
        private readonly SQSJobQueueService _queueService = new();

        public JobConsumerService(ILogger<JobConsumerService> logger, IOptions<AppConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueService.ReceiveAsync(stoppingToken);
                if (message == null)
                    continue;
                    
                var task = JsonSerializer.Deserialize<TaskAssigned>(message.Body);

                bool succeded = await ExecuteCommandAsync(task.Command);
                if (succeded)
                    await _queueService.DequeueAsync(message, stoppingToken);
            }            
        }

        private async Task<bool> ExecuteCommandAsync(string command)
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

            process.WaitForExit();

            // TODO: record metadata
            Logger.Instance.Info($"ExitCode: {process.ExitCode}, ExitTime: {process.ExitTime}");

            //process.ExitCode
            //process.ExitTime
            //var output = await process.StandardOutput.ReadToEndAsync();
            //var errors = await process.StandardError.ReadToEndAsync();

            return true;
        }
    }
}