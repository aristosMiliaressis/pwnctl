using pwnctl.app;
using pwnctl.app.Assets;
using pwnctl.app.Queueing.DTO;
using pwnctl.infra;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.app.Operations;
using System.Diagnostics;

namespace pwnctl.proc
{
    public sealed class OutputProcessorService : LifetimeService
    {
        private readonly AssetProcessor _processor = new();
        private readonly OperationManager _opManager = new(new OperationDbRepository(), 
                                                        new TaskDbRepository(),
                                                        new EventBridgeClient());

        public OutputProcessorService(IHostApplicationLifetime svcLifetime) : base(svcLifetime) { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("PWNCTL_Operation"), out int opId))
            {
                await _opManager.TryHandleAsync(opId);
                await StopAsync(stoppingToken);
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
            }

            await StopAsync(stoppingToken);
        }

        private async Task ProcessOutputBatchAsync(CancellationToken stoppingToken)
        {
            OutputBatchDTO? batchDTO = null;
            try 
            {
                batchDTO = await PwnInfraContext.TaskQueueService.ReceiveAsync<OutputBatchDTO>(stoppingToken);
                if (batchDTO is null)
                {
                    PwnInfraContext.Logger.Information("output queue is empty, sleeping for 5 secs.");
                    Thread.Sleep(5000);
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            PwnInfraContext.Logger.Information($"Received output batch #{batchDTO.TaskId} with {batchDTO.Lines.Count} lines.");
            var stopWatch = Stopwatch.StartNew();

            try
            {
                foreach (var line in batchDTO.Lines)
                {
                    PwnInfraContext.Logger.Debug("Processing: " + line);

                    await _processor.TryProcessAsync(line, batchDTO.TaskId);
                }

                await PwnInfraContext.TaskQueueService.DequeueAsync(batchDTO);

                stopWatch.Stop();
                PwnInfraContext.Logger.Information($"Processed output batch #{batchDTO.TaskId} in {stopWatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);

                // return the task to the queue, if this occures to many times,
                // the task will be put in the dead letter queue
                await PwnInfraContext.TaskQueueService.ChangeMessageVisibilityAsync(batchDTO, 0);
            }
        }
    }
}
