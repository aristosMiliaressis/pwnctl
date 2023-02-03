using pwnctl.infra.Commands;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app;
using pwnctl.app.Queueing.DTO;

namespace pwnctl.infra.Queueing
{
    public sealed class BashTaskQueueService : TaskQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(PwnInfraContext.Config.InstallPath , "queue/");

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task<bool> EnqueueAsync(QueuedTaskDTO task, CancellationToken token = default)
        {
            await CommandExecutor.ExecuteAsync("job-queue.sh", $"-w {PwnInfraContext.Config.TaskQueue.WorkerCount} -q {_queueDirectory}", task.Command, token);

            return true;
        }

        public Task<QueuedTaskDTO> ReceiveAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task DequeueAsync(QueuedTaskDTO task)
        {
            throw new NotImplementedException();
        }

        public Task ChangeMessageVisibilityAsync(QueuedTaskDTO task, int visibilityTimeout, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
