using pwnwrk.domain.Common.Interfaces;
using pwnwrk.domain.Tasks.Entities;

namespace pwnwrk.infra.Queues
{
    public sealed class MockJobQueueService : JobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public Task EnqueueAsync(TaskRecord job)
        {
            //job.Queued();

            return Task.CompletedTask;
        }
    }
}
