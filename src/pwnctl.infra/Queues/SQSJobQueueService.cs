using pwnctl.core.Interfaces;
using pwnctl.infra.Logging;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pwnctl.infra.Queues
{
    public class SQSJobQueueService : IJobQueueService
    {
        private static readonly string _queueName = "pwnctl.fifo";
        //private static readonly string _dlqName = "pwnctl-dlq.fifo";
        private readonly AmazonSQSClient _sqsClient = new();

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync(core.Entities.Task job)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName);

            var task = new TaskAssigned() { Command = job.WrappedCommand };

            var request = new SendMessageRequest
            {
                MessageGroupId = "jobs",
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public async Task<Message> ReceiveAsync(CancellationToken ct)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName, ct);
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MaxNumberOfMessages = 1
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, ct);
            if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.Instance.Info($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                Logger.Instance.Info(JsonSerializer.Serialize(messageResponse));
                // TODO: error handling
            }

            return messageResponse.Messages.FirstOrDefault();
        }

        public async Task DequeueAsync(Message message, CancellationToken ct)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName, ct);

            await _sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, ct);
        }
    }

    public class TaskAssigned
    {
        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
