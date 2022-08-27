using pwnctl.infra.Configuration;
using pwnctl.infra.Queues;
using pwnctl.core.Interfaces;

namespace pwnctl.app.Utilities
{
    public static class JobQueueFactory
    {
        public static IJobQueueService Create()
        {
            if (EnvironmentVariables.PWNTAINER_SQS)
            {
                return new SQSJobQueueService();
            }
            else if (EnvironmentVariables.PWNTAINER_TEST)
            {
                return new MockJobQueueService();
            }

            return new BashJobQueueService();
        }
    }
}
