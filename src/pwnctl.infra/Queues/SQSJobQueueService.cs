using System;
using System.Diagnostics;
using pwnctl.core.Interfaces;

namespace pwnctl.infra.Queues
{
    public class SQSJobQueueService : IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(core.Entities.Task job)
        {
            throw new NotImplementedException();
        }
    }
}
