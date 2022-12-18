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
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly SQSTaskQueueService _queueService = new();
        private readonly PwnctlDbContext _context = new();
        private CancellationTokenSource _cts = new();
        
        public TaskConsumerService(IHostApplicationLifetime hostApplicationLifetime)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnContext.Logger.Information($"{nameof(TaskConsumerService)} started.");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            while (!_cts.Token.IsCancellationRequested)
            {
                var messages = await _queueService.ReceiveAsync(_cts.Token);
                if (messages == null || !messages.Any())
                {
                    PwnContext.Logger.Information($"queue is empty");
                    break;
                }

                var message = messages[0];

                var timer = new System.Timers.Timer(1000 * (AwsConstants.QueueVisibilityTimeoutInSec - 10));
                timer.Elapsed += async (sender, e) => await _queueService.ChangeBatchVisibility(new List<Message> { message }, _cts.Token);
                timer.Start();

                var queuedTask = Serializer.Instance.Deserialize<TaskEntity>(message.Body);

                var taskRecord = await _context
                                    .JoinedTaskRecordQueryable()
                                    .FirstOrDefaultAsync(r => r.Id == queuedTask.TaskId);

                Process process = null;
                try
                {
                    taskRecord.Started();

                    process = await ExecuteCommandAsync(queuedTask.Command, taskRecord.Definition, _cts.Token);
                }
                catch (TaskCanceledException ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    continue;
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Debug(message.Body);
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                    await _queueService.DequeueAsync(message);
                    continue;
                }

                // TODO: log stdout&stderr if ExitCode != 0
                taskRecord.Finished(process.ExitCode);

                await _context.SaveChangesAsync();

                await _queueService.DequeueAsync(message);
            }
            
            _hostApplicationLifetime.StopApplication();
        }

        private async Task<Process> ExecuteCommandAsync(string command, TaskDefinition definition, CancellationToken stoppingToken)
        {
            PwnContext.Logger.Debug("Running: " + command);
            
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

        private void OnProcessExit(object sender, EventArgs e)
        {
            PwnContext.Logger.Information("received SIGTERM");

            _cts.Cancel();
        }
    }
}