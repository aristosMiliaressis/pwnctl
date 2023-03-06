using System.Text;
using pwnctl.app;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Queueing
{
    public sealed class MockTaskQueueService : TaskQueueService
    {
        private static readonly string _queuePath = "./queue";

        public MockTaskQueueService()
        {
            CommandExecutor.ExecuteAsync($"touch {_queuePath}").Wait();
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public async Task<bool> EnqueueAsync(QueuedTaskDTO task, CancellationToken token = default)
        {
            var json = PwnInfraContext.Serializer.Serialize(task);
            
            await CommandExecutor.ExecuteAsync($"echo '{json}' >> {_queuePath}");
            
            return true;
        }

        public async Task<QueuedTaskDTO> ReceiveAsync(CancellationToken token = default)
        {
            (_, StringBuilder stdout, _) = await CommandExecutor.ExecuteAsync($"if read line <{_queuePath}; then echo $line; tmp=\"$(tail -n +2 {_queuePath})\"; echo \"$tmp\" > {_queuePath}; fi");

            if (string.IsNullOrEmpty(stdout.ToString()))
                return null;

            return PwnInfraContext.Serializer.Deserialize<QueuedTaskDTO>(stdout.ToString());
        }

        public Task DequeueAsync(QueuedTaskDTO task)
        {
            return Task.CompletedTask;
        }

        public Task ChangeMessageVisibilityAsync(QueuedTaskDTO task, int visibilityTimeout, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
