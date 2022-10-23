using pwnwrk.infra;
using pwnwrk.infra.Queues;
using pwnwrk.domain.Interfaces;

namespace pwnwrk.cli.Utilities
{
    public static class JobQueueFactory
    {
        public static IJobQueueService Create()
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
