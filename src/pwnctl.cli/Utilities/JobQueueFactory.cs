using pwnwrk.infra.Configuration;
using pwnwrk.infra.Queues;
using pwnwrk.domain.Interfaces;

namespace pwnctl.cli.Utilities
{
    public static class JobQueueFactory
    {
        public static IJobQueueService Create()
        {
            if (ConfigurationManager.Config.JobQueue.IsSQS)
            {
                return new SQSJobQueueService();
            }
            else if (ConfigurationManager.Config.IsTestRun)
            {
                return new MockJobQueueService();
            }

            return new BashJobQueueService();
        }
    }
}
