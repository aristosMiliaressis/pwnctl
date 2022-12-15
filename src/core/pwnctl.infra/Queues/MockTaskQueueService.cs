using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.infra.Queues
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public Task EnqueueAsync(TaskRecord task)
        {
            //task.Queued();

            return Task.CompletedTask;
        }
    }
}
