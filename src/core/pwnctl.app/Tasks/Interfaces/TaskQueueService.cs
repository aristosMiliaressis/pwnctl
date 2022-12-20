using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.DTO;

namespace pwnctl.app.Tasks.Interfaces
{
    public interface TaskQueueService
    {
        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        Task<bool> EnqueueAsync(TaskDTO task, CancellationToken token = default);
        Task DequeueAsync(TaskDTO task, CancellationToken token = default);
        Task<List<TaskDTO>> ReceiveAsync(CancellationToken token);
        Task ChangeBatchVisibility(List<TaskDTO> tasks, CancellationToken token);
    }
}
