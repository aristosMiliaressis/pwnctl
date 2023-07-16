using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Notifications.Enums;
using pwnctl.infra;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;
using pwnctl.app.Operations;
using pwnctl.infra.Configuration;

namespace pwnctl.proc
{
    public sealed class OutputProcessorService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private static readonly AssetProcessor _processor = AssetProcessorFactory.Create();
        private static readonly TaskQueueService _queueService = TaskQueueServiceFactory.Create();
        private static System.Timers.Timer _timer = new();
        private static readonly OperationInitializer _initializer = new(new OperationDbRepository(),
                                                                new AssetDbRepository(),
                                                                new TaskDbRepository(),
                                                                _queueService);

        public OutputProcessorService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                PwnInfraContext.NotificationSender.Send($"{nameof(OutputProcessorService)} stoped.", NotificationTopic.Status);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PwnInfraContext.NotificationSender.Send($"{nameof(OutputProcessorService)}:{EnvironmentVariables.IMAGE_HASH} started.", NotificationTopic.Status);

            if (int.TryParse(Environment.GetEnvironmentVariable("PWNCTL_Operation"), out int opId))
            {
                await _initializer.InitializeAsync(opId);
                _hostApplicationLifetime.StopApplication();
                return;
            }

            var rng = new Random();

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
            var batchDTO = await _queueService.ReceiveAsync<OutputBatchDTO>(stoppingToken);
            if (batchDTO == null)
            {
                PwnInfraContext.Logger.Information("no work found");
                return;
            }

            // Change the message visibility if the visibility window is exheeded
            // this allows us to keep a smaller visibility window without effecting
            // the max task timeout.
            _timer = new System.Timers.Timer((PwnInfraContext.Config.TaskQueue.VisibilityTimeout - 90) * 1000);
            _timer.Elapsed += async (_, _) =>
                await _queueService.ChangeMessageVisibilityAsync(batchDTO, PwnInfraContext.Config.TaskQueue.VisibilityTimeout);
            _timer.Start();

            try
            {
                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: " + line);

                    await _processor.TryProcessAsync(line, batchDTO.TaskId);
                }

                await _queueService.DequeueAsync(batchDTO);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await _queueService.ChangeMessageVisibilityAsync(batchDTO, 0);
            }
            finally
            {
                _timer.Stop();
            }
        }
    }
}
