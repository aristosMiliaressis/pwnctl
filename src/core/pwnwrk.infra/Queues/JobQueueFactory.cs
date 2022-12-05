using pwnwrk.domain.Common.Interfaces;

namespace pwnwrk.infra.Queues
{
    public static class JobQueueFactory
    {
        public static JobQueueService Create()
        {
            if (PwnContext.Config.IsTestRun)
            {
                return new MockJobQueueService();
            }
            else if (PwnContext.Config.JobQueue.UseBash)
            {
                return new BashJobQueueService();
            }

            return new SQSJobQueueService();
        }
    }
}
