using System.Text;
using pwnctl.app;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Commands;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Queueing
{
    public sealed class FakeTaskQueueService : TaskQueueService
    {
        private static readonly string _queuePath = Path.Combine(Directory.GetCurrentDirectory(), "queue");
        private static CommandExecutor _executor = new BashCommandExecutor();

        public FakeTaskQueueService()
        {
            _executor.ExecuteAsync($"touch {_queuePath}").Wait();
            _executor.ExecuteAsync($"chmod 666 {_queuePath}").Wait();
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="task"></param>
        public async Task EnqueueAsync<TMessage>(TMessage task)
            where TMessage : QueueMessage
        {
            var json = PwnInfraContext.Serializer.Serialize(task);

            await _executor.ExecuteAsync($"echo '{json}' >> {_queuePath}");
        }

        public async Task EnqueueBatchAsync<TMessage>(IEnumerable<TMessage> tasks)
            where TMessage : QueueMessage
        {
            foreach (var task in tasks)
            {
                await EnqueueAsync(task);
            }
        }

        public async Task<TMessage> ReceiveAsync<TMessage>(CancellationToken token = default)
            where TMessage : QueueMessage
        {
            (_, StringBuilder stdout, _) = await _executor.ExecuteAsync($"if read line <{_queuePath}; then echo $line; tmp=\"$(tail -n +2 {_queuePath})\"; echo \"$tmp\" > {_queuePath}; fi");

            if (string.IsNullOrEmpty(stdout.ToString().Trim()))
                return default(TMessage);

            return PwnInfraContext.Serializer.Deserialize<TMessage>(stdout.ToString());
        }

        public Task DequeueAsync(QueueMessage task)
        {
            return Task.CompletedTask;
        }

        public Task ChangeMessageVisibilityAsync(QueueMessage task, int visibilityTimeout)
        {
            return Task.CompletedTask;
        }

        public Task Purge<TMessage>()
        {
            return Task.CompletedTask;
        }
    }
}
