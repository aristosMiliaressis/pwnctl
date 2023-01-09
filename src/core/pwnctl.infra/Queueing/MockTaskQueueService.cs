using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;

namespace pwnctl.infra.Queueing
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        public Task ChangeBatchVisibility(List<QueueTaskDTO> tasks, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task DequeueAsync(QueueTaskDTO task, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task<bool> EnqueueAsync(QueueTaskDTO task, CancellationToken token = default)
        {
            return Task.FromResult(false);
        }

        public Task<List<QueueTaskDTO>> ReceiveAsync(CancellationToken token = default)
        {
            return Task.FromResult(new List<QueueTaskDTO>());
        }
    }
}
