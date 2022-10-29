using pwnwrk.domain.Interfaces;
using pwnwrk.infra.Configuration;
using pwnwrk.infra.Aws;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Core;

namespace pwnwrk.infra.Queues
{
    public class SQSJobQueueService : IJobQueueService
    {
        private readonly AmazonSQSClient _sqsClient = new();

        private Dictionary<string,string> _queueUrls;
        private string this[string queueName]
        {
            get
            {
                if (_queueUrls == null)
                    _queueUrls = new();

                if (!_queueUrls.TryGetValue(queueName, out string queueUrl))
                {
                    var queueUrlResponse = _sqsClient.GetQueueUrlAsync(AwsConstants.QueueName).Result;
                    queueUrl = queueUrlResponse.QueueUrl;
                    _queueUrls[queueName] = queueUrl;
                }

                return queueUrl;
            }
        }

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync(domain.Entities.Task job)
        {
            PwnContext.Logger.Debug("Enqueue: " + job.WrappedCommand);

            var task = new TaskAssigned() 
            { 
                TaskId = job.Id,
                Command = job.WrappedCommand
            };

            var request = new SendMessageRequest
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = JsonSerializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public async Task<List<Message>> ReceiveAsync(CancellationToken ct)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                MaxNumberOfMessages = 10
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, ct);
            if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                PwnContext.Logger.Warning(JsonSerializer.Serialize(messageResponse));
                PwnContext.Logger.Warning($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                // TODO: error handling
            }

            return messageResponse.Messages;
        }

        public async Task DequeueAsync(Message message, CancellationToken ct)
        {
            // TODO: delete batching?

            var response = await _sqsClient.DeleteMessageAsync(this[AwsConstants.QueueName], message.ReceiptHandle, ct);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnContext.Logger.Debug("DeleteMessage: " + JsonSerializer.Serialize(response));
        }

        public async Task ChangeBatchVisibility(List<Message> messages, CancellationToken ct)
        {
            // TODO: what if timeout exhedded more than once?

            var request = new ChangeMessageVisibilityBatchRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                Entries = messages.Select(msg => new ChangeMessageVisibilityBatchRequestEntry 
                {
                    Id = msg.MessageId,
                    ReceiptHandle = msg.ReceiptHandle,
                    VisibilityTimeout = AwsConstants.QueueVisibilityTimeoutInSec *2*60
                }).ToList()
            };

            var response = await _sqsClient.ChangeMessageVisibilityBatchAsync(request, ct);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnContext.Logger.Debug("ChangeMessageVisibility: " + JsonSerializer.Serialize(response));
        }
    }

    public class TaskAssigned
    {
        [JsonPropertyName("taskId")]
        public int TaskId { get; init; }

        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
