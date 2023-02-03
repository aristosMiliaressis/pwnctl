using pwnctl.app.Queueing.DTO;

namespace pwnctl.app.Queueing.Interfaces;

public interface TaskQueueService
{
    /// <summary>
    /// pushes a task to the pending queue.
    /// </summary>
    /// <param name="task"></param>
    Task<bool> EnqueueAsync(QueuedTaskDTO task, CancellationToken token = default);
    Task<QueuedTaskDTO> ReceiveAsync(CancellationToken token = default);
    Task DequeueAsync(QueuedTaskDTO task);
    Task ChangeMessageVisibilityAsync(QueuedTaskDTO task, int visibilityTimeout, CancellationToken token = default);
}

