using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Models;

namespace pwnctl.app.Tasks.Interfaces
{
    public interface TaskQueueService
    {
        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        Task EnqueueAsync(TaskRecord task, CancellationToken token = default);
        Task DequeueAsync(QueueMessage message, CancellationToken token = default);
        Task<List<QueueMessage>> ReceiveAsync(CancellationToken token);
        Task ChangeBatchVisibility(List<QueueMessage> messages, CancellationToken token);
    }
}
