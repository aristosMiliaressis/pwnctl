using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.core.Interfaces;

namespace pwnctl.app.Utilities
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
