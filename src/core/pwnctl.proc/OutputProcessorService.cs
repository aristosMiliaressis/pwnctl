using System.Threading;
using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Enums;
using pwnctl.infra;
using pwnctl.infra.DependencyInjection;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;
using pwnctl.app.Operations;
using pwnctl.infra.Configuration;

namespace pwnctl.proc
{
    public sealed class OutputProcessorService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private static readonly AssetProcessor _processor = new();
        private static System.Timers.Timer _timer = new();
        private static readonly OperationInitializer _initializer = new(new OperationDbRepository());

        public OutputProcessorService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                PwnInfraContext.NotificationSender.SendAsync($"{nameof(OutputProcessorService)} stoped.", NotificationTopic.Status).Wait();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await PwnInfraContext.NotificationSender.SendAsync($"{nameof(OutputProcessorService)}:{EnvironmentVariables.IMAGE_HASH} started.", NotificationTopic.Status);

            try
            {
                if (int.TryParse(Environment.GetEnvironmentVariable("PWNCTL_Operation"), out int opId))
                {
                    await _initializer.InitializeAsync(opId);
                    _hostApplicationLifetime.StopApplication();
                    return;
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutputBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    PwnInfraContext.Logger.Exception(ex);
                }
                finally
                {
                    _timer.Dispose();
                }
            }
        }

        private async Task ProcessOutputBatchAsync(CancellationToken stoppingToken)
        {
            var batchDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<OutputBatchDTO>(stoppingToken);
            if (batchDTO is null)
            {
                PwnInfraContext.Logger.Information("no work found");
                Thread.Sleep(5000);
                return;
            }

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            _timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            _timer.Elapsed += async (_, _) =>
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(batchDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            _timer.Start();

            try
            {
                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: " + line);

                    await _processor.TryProcessAsync(line, batchDTO.TaskId);
                }

                await PwnInfraContext.TaskQueueService.DequeueAsync(batchDTO);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(batchDTO, 0);
            }
            finally
            {
                _timer.Stop();
            }
        }
    }
}
