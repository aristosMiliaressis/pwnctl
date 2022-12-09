using pwnctl.app.Entities;

namespace pwnctl.app.Interfaces
{
    public interface JobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(TaskRecord job);
    }
}
