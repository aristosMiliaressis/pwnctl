using pwnwrk.infra;
using pwnwrk.infra.Queues;
using pwnwrk.domain.Interfaces;

namespace pwnctl.cli.Utilities
{
    public static class JobQueueFactory
    {
        public static IJobQueueService Create()
        {
            if (PwnContext.Config.JobQueue.IsSQS)
            {
                return new SQSJobQueueService();
            }
            else if (PwnContext.Config.IsTestRun)
            {
                return new MockJobQueueService();
            }

            return new BashJobQueueService();
        }
    }
}
