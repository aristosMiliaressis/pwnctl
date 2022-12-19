using System.Diagnostics;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Models;

namespace pwnctl.infra.Queues
{
    public sealed class BashTaskQueueService : TaskQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(PwnContext.Config.InstallPath , "queue/");

        public Task ChangeBatchVisibility(List<QueueMessage> messages, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task DequeueAsync(QueueMessage message, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync(TaskRecord task, CancellationToken token = default)
        {
            await CommandExecutor.ExecuteAsync("job-queue.sh", $"-w {PwnContext.Config.TaskQueue.WorkerCount} -q {_queueDirectory}", token);

            task.Queued();
        }

        public Task<List<QueueMessage>> ReceiveAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
