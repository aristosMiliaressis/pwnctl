﻿using pwnctl.core.Interfaces;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pwnctl.infra.Queues
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
                    var queueUrlResponse = _sqsClient.GetQueueUrlAsync(ConfigurationManager.Config.JobQueue.QueueName).Result;
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
        public async Task EnqueueAsync(core.Entities.Task job)
        {
            var task = new TaskAssigned() { Command = job.WrappedCommand };

            var request = new SendMessageRequest
            {
                MessageGroupId = ConfigurationManager.Config.JobQueue.MessageGroup,
                QueueUrl = this[ConfigurationManager.Config.JobQueue.QueueName],
                MessageBody = JsonSerializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public async Task<List<Message>> ReceiveAsync(CancellationToken ct)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[ConfigurationManager.Config.JobQueue.QueueName],
                MaxNumberOfMessages = 10
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, ct);
            Logger.Instance.Info(JsonSerializer.Serialize(messageResponse));
            if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.Instance.Info($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                // TODO: error handling
            }

            return messageResponse.Messages;
        }

        public async Task DequeueAsync(Message message, CancellationToken ct)
        {
            // TODO: delete batching?

            var response = await _sqsClient.DeleteMessageAsync(this[ConfigurationManager.Config.JobQueue.QueueName], message.ReceiptHandle, ct);

            Logger.Instance.Info("DeleteMessage: " + JsonSerializer.Serialize(response));
        }

        public async Task ChangeBatchVisibility(List<Message> messages, CancellationToken ct)
        {
            // TODO: what if timeout exhedded more than once?

            var request = new ChangeMessageVisibilityBatchRequest
            {
                QueueUrl = this[ConfigurationManager.Config.JobQueue.QueueName],
                Entries = messages.Select(msg => new ChangeMessageVisibilityBatchRequestEntry 
                {
                    Id = msg.MessageId,
                    ReceiptHandle = msg.ReceiptHandle,
                    VisibilityTimeout = ConfigurationManager.Config.JobQueue.VisibilityTimeout*2*60
                }).ToList()
            };

            var response = await _sqsClient.ChangeMessageVisibilityBatchAsync(request, ct);

            Logger.Instance.Info("ChangeMessageVisibility: " + JsonSerializer.Serialize(response));
        }
    }

    public class TaskAssigned
    {
        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
