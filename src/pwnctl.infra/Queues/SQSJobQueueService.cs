using pwnctl.core.Interfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pwnctl.infra.Queues
{
    public class SQSJobQueueService : IJobQueueService
    {
        private readonly AmazonSQSClient _sqsClient = new();

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync(core.Entities.Task job)
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync("jobs");

            var task = new TaskAssigned() { Command = job.WrappedCommand };

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request);
        }
    }

    public class TaskAssigned
    {
        [JsonPropertyName("command")]
        public string Command { get; init; }
    }
}
