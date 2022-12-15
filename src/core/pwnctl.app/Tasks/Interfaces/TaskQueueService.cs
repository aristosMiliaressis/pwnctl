using pwnctl.app.Tasks.Entities;

namespace pwnctl.app.Tasks.Interfaces
{
    public interface TaskQueueService
    {
        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(TaskRecord task);
    }
}
