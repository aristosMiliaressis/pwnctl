using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.core.Interfaces;

namespace pwnctl.app.Utilities
{
    public static class JobQueueFactory
    {
        public static IJobQueueService Create()
        {
            if (EnvironmentVariables.PWNCTL_SQS)
            {
                return new SQSJobQueueService();
            }
            else if (EnvironmentVariables.PWNCTL_TEST)
            {
                return new MockJobQueueService();
            }

            return new BashJobQueueService();
        }
    }
}
