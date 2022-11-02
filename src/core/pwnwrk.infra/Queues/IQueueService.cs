using pwnwrk.domain.Tasks.Entities;

namespace pwnwrk.infra.Queues
{
    public interface IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(TaskRecord job);
    }
}
