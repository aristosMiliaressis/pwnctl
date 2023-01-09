using pwnctl.infra.Aws;
using Amazon.SQS;
using Amazon.SQS.Model;
using pwnctl.app;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Queueing.DTO;

namespace pwnctl.infra.Queueing
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
        public async Task<bool> EnqueueAsync(QueueTaskDTO task, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug("Enqueue: " + task.Command);

            var request = new SendMessageRequest
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = PwnInfraContext.Serializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request, token);

            return true;
        }

        public async Task<List<QueueTaskDTO>> ReceiveAsync(CancellationToken token = default)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                MaxNumberOfMessages = 1
            };

            var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, token);
            if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(messageResponse));
                PwnInfraContext.Logger.Warning($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                // TODO: error handling
            }

            return messageResponse.Messages.Select(msg => 
            {
                var task = PwnInfraContext.Serializer.Deserialize<QueueTaskDTO>(msg.Body);
                
                task.Metadata = new Dictionary<string, string>
                {
                    { nameof(msg.MessageId), msg.MessageId },
                    { nameof(msg.ReceiptHandle), msg.ReceiptHandle }
                };

                return task;
            }).ToList();
        }

        public async Task DequeueAsync(QueueTaskDTO task, CancellationToken token = default)
        {
            // TODO: delete batching?

            var response = await _sqsClient.DeleteMessageAsync(this[AwsConstants.QueueName], task.Metadata[nameof(Message.ReceiptHandle)], token);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnInfraContext.Logger.Debug("DeleteMessage: " + PwnInfraContext.Serializer.Serialize(response));
        }

        public async Task ChangeBatchVisibility(List<QueueTaskDTO> tasks, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug($"ChangeBatchVisibility fot tasks {string.Join(",", tasks.Select(t=>t.TaskId))}");

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
                PwnInfraContext.Logger.Debug("ChangeMessageVisibility: " + PwnInfraContext.Serializer.Serialize(response));
        }
    }
}
