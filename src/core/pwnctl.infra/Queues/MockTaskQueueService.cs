using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Models;

namespace pwnctl.infra.Queues
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        public Task ChangeBatchVisibility(List<QueueMessage> messages, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task DequeueAsync(QueueMessage message, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(TaskRecord task, CancellationToken token = default)
        {
            //task.Queued();

            return Task.CompletedTask;
        }

        public Task<List<QueueMessage>> ReceiveAsync(CancellationToken token = default)
        {
            return Task.FromResult(new List<QueueMessage>());
        }
    }
}
