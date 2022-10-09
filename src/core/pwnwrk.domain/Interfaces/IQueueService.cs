using System;
using System.Diagnostics;

namespace pwnwrk.domain.Interfaces
{
    public interface IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(domain.Entities.Task job);
    }
}
