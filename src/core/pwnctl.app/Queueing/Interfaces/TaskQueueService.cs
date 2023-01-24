using pwnctl.app.Queueing.DTO;

namespace pwnctl.app.Queueing.Interfaces;

public interface TaskQueueService
{
    /// <summary>
    /// pushes a task to the pending queue.
    /// </summary>
    /// <param name="task"></param>
    Task<bool> EnqueueAsync(QueueTaskDTO task, CancellationToken token = default);
    Task<QueueTaskDTO> ReceiveAsync(CancellationToken token = default);
    Task DequeueAsync(QueueTaskDTO task);
    Task ChangeMessageVisibilityAsync(QueueTaskDTO task, int visibilityTimeout, CancellationToken token = default);
}

