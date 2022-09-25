﻿using pwnctl.core.Interfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pwnctl.infra.Queues
{
    public class SQSJobQueueService : IJobQueueService
    {
        private static readonly string _queueName = "jobs";
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
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public async Task<string> DequeueAsync()
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName);
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MaxNumberOfMessages = 1
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, CancellationToken.None);
            return messageResponse.Messages.FirstOrDefault()?.Body;
        }
    }

    public class TaskAssigned
    {
        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
