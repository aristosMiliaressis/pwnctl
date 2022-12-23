using pwnctl.infra.Aws;
using Amazon.SQS;
using Amazon.SQS.Model;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Tasks.DTO;

namespace pwnctl.infra.Queues
{
    public sealed class SQSTaskQueueService : TaskQueueService
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
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task<bool> EnqueueAsync(TaskDTO task, CancellationToken token = default)
        {
            PwnContext.Logger.Debug("Enqueue: " + task.Command);

            var taskEnt = new TaskDTO() 
            { 
                TaskId = task.TaskId,
                Command = task.Command
            };

            var request = new SendMessageRequest
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = Serializer.Instance.Serialize(taskEnt)
            };

            await _sqsClient.SendMessageAsync(request, token);

            return true;
        }

        public async Task<List<TaskDTO>> ReceiveAsync(CancellationToken token = default)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                MaxNumberOfMessages = 1
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, token);
            if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                PwnContext.Logger.Warning(Serializer.Instance.Serialize(messageResponse));
                PwnContext.Logger.Warning($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                // TODO: error handling
            }

            return messageResponse.Messages.Select(msg => 
            {
                var task = Serializer.Instance.Deserialize<TaskDTO>(msg.Body);
                
                task.Metadata = new Dictionary<string, string>
                {
                    { nameof(msg.MessageId), msg.MessageId },
                    { nameof(msg.ReceiptHandle), msg.ReceiptHandle }
                };

                return task;
            }).ToList();
        }

        public async Task DequeueAsync(TaskDTO task, CancellationToken token = default)
        {
            // TODO: delete batching?

            var response = await _sqsClient.DeleteMessageAsync(this[AwsConstants.QueueName], task.Metadata[nameof(Message.ReceiptHandle)], token);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnContext.Logger.Debug("DeleteMessage: " + Serializer.Instance.Serialize(response));
        }

        public async Task ChangeBatchVisibility(List<TaskDTO> tasks, CancellationToken token = default)
        {
            PwnContext.Logger.Debug($"ChangeBatchVisibility fot tasks {string.Join(",", tasks.Select(t=>t.TaskId))}");

            // TODO: what if timeout exhedded more than once?

            var request = new ChangeMessageVisibilityBatchRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                Entries = tasks.Select(msg => new ChangeMessageVisibilityBatchRequestEntry 
                {
                    Id = msg.Metadata[nameof(Message.MessageId)],
                    ReceiptHandle = msg.Metadata[nameof(Message.ReceiptHandle)],
                    VisibilityTimeout = AwsConstants.QueueVisibilityTimeoutInSec *2*60
                }).ToList()
            };

            var response = await _sqsClient.ChangeMessageVisibilityBatchAsync(request, token);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnContext.Logger.Debug("ChangeMessageVisibility: " + Serializer.Instance.Serialize(response));
        }
    }
}
