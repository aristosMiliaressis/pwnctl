
namespace pwnwrk.infra.Queues
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
