namespace pwnctl.infra.Queues;

using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Configuration;

public static class TaskQueueServiceFactory
{
    public static TaskQueueService Create()
    {
        return EnvironmentVariables.IsTestRun
            ? new MockTaskQueueService()
            : new SQSTaskQueueService();
    }
}
