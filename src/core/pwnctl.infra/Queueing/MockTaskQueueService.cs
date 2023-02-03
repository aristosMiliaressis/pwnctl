using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;

namespace pwnctl.infra.Queueing
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task<bool> EnqueueAsync(QueuedTaskDTO task, CancellationToken token = default)
        {
            return Task.FromResult(false);
        }

        public Task<QueuedTaskDTO> ReceiveAsync(CancellationToken token = default)
        {
            return Task.FromResult(new QueuedTaskDTO());
        }

        public Task DequeueAsync(QueuedTaskDTO task)
        {
            return Task.CompletedTask;
        }

        public Task ChangeMessageVisibilityAsync(QueuedTaskDTO task, int visibilityTimeout, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
