using System.Text;
using pwnctl.app;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Commands;

namespace pwnctl.infra.Queueing
{
    public sealed class FakeTaskQueueService : TaskQueueService
    {
        private static readonly string _queuePath = "./queue";

        public FakeTaskQueueService()
        {
            CommandExecutor.ExecuteAsync($"touch {_queuePath}").Wait();
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public async Task EnqueueAsync<TMessage>(TMessage task, CancellationToken token = default)
            where TMessage : QueueMessage
        {
            var json = PwnInfraContext.Serializer.Serialize(task);

            await CommandExecutor.ExecuteAsync($"echo '{json}' >> {_queuePath}");
        }

        public async Task<TMessage> ReceiveAsync<TMessage>(CancellationToken token = default)
            where TMessage : QueueMessage
        {
            (_, StringBuilder stdout, _) = await CommandExecutor.ExecuteAsync($"if read line <{_queuePath}; then echo $line; tmp=\"$(tail -n +2 {_queuePath})\"; echo \"$tmp\" > {_queuePath}; fi");

            if (string.IsNullOrEmpty(stdout.ToString()))
                return default(TMessage);

            return PwnInfraContext.Serializer.Deserialize<TMessage>(stdout.ToString());
        }

        public Task DequeueAsync(QueueMessage task)
        {
            return Task.CompletedTask;
        }

        public Task ChangeMessageVisibilityAsync(QueueMessage task, int visibilityTimeout, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
