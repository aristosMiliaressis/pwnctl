using pwnctl.infra.Commands;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.DTO;
using pwnctl.app;

namespace pwnctl.infra.Queues
{
    public sealed class BashTaskQueueService : TaskQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(PwnInfraContext.Config.InstallPath , "queue/");

        public Task ChangeBatchVisibility(List<TaskDTO> tasks, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task DequeueAsync(TaskDTO task, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task<bool> EnqueueAsync(TaskDTO task, CancellationToken token = default)
        {
            await CommandExecutor.ExecuteAsync("job-queue.sh", $"-w {PwnInfraContext.Config.TaskQueue.WorkerCount} -q {_queueDirectory}", task.Command, token);

            return true;
        }

        public Task<List<TaskDTO>> ReceiveAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
