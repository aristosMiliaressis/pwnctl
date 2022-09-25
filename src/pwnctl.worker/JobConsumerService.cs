using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using System.Diagnostics;
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
                var command = await _queueService.DequeueAsync();
                await ExecuteCommandAsync(command);
            }            
        }

        private async Task ExecuteCommandAsync(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            {
                if (process == null)
                    return;

                using (StreamWriter sr = process.StandardInput)
                {
                    await sr.WriteLineAsync(command);
                    sr.Flush();
                    sr.Close();
                }
            }

            process.WaitForExit();

            // TODO: record metadata
            //process.ExitCode
            //process.ExitTime
            //var output = await process.StandardOutput.ReadToEndAsync();
            //var errors = await process.StandardError.ReadToEndAsync();
        }
    }
}