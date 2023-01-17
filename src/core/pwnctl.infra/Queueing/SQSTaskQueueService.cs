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
            PwnInfraContext.Logger.Debug($"Enqueue: {task.TaskId} : {task.Command}");

            var request = new SendMessageRequest
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = PwnInfraContext.Serializer.Serialize(task)
            };

            await _sqsClient.SendMessageAsync(request, token);

            return true;
        }

        public async Task<QueueTaskDTO> ReceiveAsync(CancellationToken token = default)
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
            }

            return messageResponse.Messages.Select(msg => 
            {
                var task = PwnInfraContext.Serializer.Deserialize<QueueTaskDTO>(msg.Body);
                PwnInfraContext.Logger.Debug($"Received : {task.TaskId}, MessageId: {msg.MessageId}, ReceiptHandle: {msg.ReceiptHandle}");

                task.Metadata = new Dictionary<string, string>
                {
                    { nameof(msg.MessageId), msg.MessageId },
                    { nameof(msg.ReceiptHandle), msg.ReceiptHandle }
                };

                return task;
            }).FirstOrDefault();
        }

        public async Task DequeueAsync(QueueTaskDTO task)
        {
            PwnInfraContext.Logger.Debug($"Dequeueing : {task.TaskId}");

            var response = await _sqsClient.DeleteMessageAsync(this[AwsConstants.QueueName], task.Metadata[nameof(Message.ReceiptHandle)], CancellationToken.None);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                PwnInfraContext.Logger.Debug("DeleteMessage: " + PwnInfraContext.Serializer.Serialize(response));
        }
    }
}
