namespace pwnctl.infra.Queueing;

using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Configuration;

public static class TaskQueueServiceFactory
{
    public static TaskQueueService Create()
    {
        return EnvironmentVariables.TEST_RUN
            ? new FakeTaskQueueService()
            : new SQSTaskQueueService();
    }
}
