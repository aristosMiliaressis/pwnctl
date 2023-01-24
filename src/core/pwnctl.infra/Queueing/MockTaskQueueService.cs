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
        public Task<bool> EnqueueAsync(QueueTaskDTO task, CancellationToken token = default)
        {
            return Task.FromResult(false);
        }

        public Task<QueueTaskDTO> ReceiveAsync(CancellationToken token = default)
        {
            return Task.FromResult(new QueueTaskDTO());
        }

        public Task DequeueAsync(QueueTaskDTO task)
        {
            return Task.CompletedTask;
        }

        public Task ChangeMessageVisibilityAsync(QueueTaskDTO task, int visibilityTimeout, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
