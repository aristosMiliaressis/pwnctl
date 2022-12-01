using System.Diagnostics;
using pwnwrk.domain.Tasks.Entities;

namespace pwnwrk.infra.Queues
{
    public sealed class MockJobQueueService : IJobQueueService
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
