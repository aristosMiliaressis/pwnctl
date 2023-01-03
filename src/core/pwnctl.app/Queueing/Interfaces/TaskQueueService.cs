using pwnctl.app.Queueing.DTO;

namespace pwnctl.app.Queueing.Interfaces;

public interface TaskQueueService
{
    /// <summary>
    /// pushes a task to the pending queue.
    /// </summary>
    /// <param name="task"></param>
    Task<bool> EnqueueAsync(QueueTaskDTO task, CancellationToken token = default);
    Task DequeueAsync(QueueTaskDTO task, CancellationToken token = default);
    Task<List<QueueTaskDTO>> ReceiveAsync(CancellationToken token);
    Task ChangeBatchVisibility(List<QueueTaskDTO> tasks, CancellationToken token);
}

