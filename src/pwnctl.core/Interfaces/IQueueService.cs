using System;
using System.Diagnostics;

namespace pwnctl.core.Interfaces
{
    public interface IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public void Enqueue(core.Entities.Task job);
    }
}