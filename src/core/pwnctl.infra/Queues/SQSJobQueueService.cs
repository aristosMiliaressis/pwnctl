using pwnctl.infra.Aws;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json.Serialization;
using pwnctl.app.Entities;
using pwnctl.app.Interfaces;

namespace pwnctl.infra.Queues
{
    public sealed class SQSJobQueueService : JobQueueService
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
        public async Task EnqueueAsync(TaskRecord job)
        {
            PwnContext.Logger.Debug("Enqueue: " + job.Command);

            var task = new TaskEntity() 
            { 
                TaskId = job.Id,
                Command = job.Command
            };

            var request = new SendMessageRequest
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = Serializer.Instance.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);

            job.Queued();
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
                PwnContext.Logger.Warning(Serializer.Instance.Serialize(messageResponse));
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
                PwnContext.Logger.Debug("DeleteMessage: " + Serializer.Instance.Serialize(response));
        }

        public async Task ChangeBatchVisibility(List<Message> messages, CancellationToken ct)
        {
            PwnContext.Logger.Debug($"ChangeBatchVisibility");

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
                PwnContext.Logger.Debug("ChangeMessageVisibility: " + Serializer.Instance.Serialize(response));
        }
    }

    public sealed class TaskEntity
    {
        [JsonPropertyName("taskId")]
        public int TaskId { get; init; }

        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
