using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.DTO;

namespace pwnctl.infra.Queues
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        public Task ChangeBatchVisibility(List<TaskDTO> tasks, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task DequeueAsync(TaskDTO task, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task<bool> EnqueueAsync(TaskDTO task, CancellationToken token = default)
        {
            return Task.FromResult(false);
        }

        public Task<List<TaskDTO>> ReceiveAsync(CancellationToken token = default)
        {
            return Task.FromResult(new List<TaskDTO>());
        }
    }
}
