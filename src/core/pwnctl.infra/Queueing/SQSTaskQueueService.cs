using pwnctl.infra.Aws;
using Amazon.SQS;
using Amazon.SQS.Model;
using pwnctl.app;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Queueing.DTO;
using System.Net;

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
        public async Task<bool> EnqueueAsync(QueuedTaskDTO task, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug($"Enqueue: {task.TaskId} : {task.Command}");

            var request = new SendMessageRequest
            {
                MessageGroupId = task.TaskId.ToString(),
                QueueUrl = this[AwsConstants.QueueName],
                MessageBody = PwnInfraContext.Serializer.Serialize(task)
            };

            try
            {
                var response = await _sqsClient.SendMessageAsync(request, token);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning($"HttpStatusCode: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }

            return true;
        }

        public async Task<QueuedTaskDTO> ReceiveAsync(CancellationToken token = default)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                MaxNumberOfMessages = 1
            };

            try
            {
                var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, token);
                if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(messageResponse));
                    PwnInfraContext.Logger.Warning($"HttpStatusCode: {messageResponse.HttpStatusCode}");
                }

                return messageResponse.Messages.Select(msg => 
                {
                    var task = PwnInfraContext.Serializer.Deserialize<QueuedTaskDTO>(msg.Body);
                    PwnInfraContext.Logger.Debug($"Received : {task.TaskId}, MessageId: {msg.MessageId}");

                    task.Metadata = new Dictionary<string, string>
                    {
                        { nameof(msg.MessageId), msg.MessageId },
                        { nameof(msg.ReceiptHandle), msg.ReceiptHandle }
                    };

                    return task;
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return null;
            }
        }

        public async Task DequeueAsync(QueuedTaskDTO task)
        {
            PwnInfraContext.Logger.Debug($"Dequeueing : {task.TaskId}");

            try
            {
                var response = await _sqsClient.DeleteMessageAsync(this[AwsConstants.QueueName], task.Metadata[nameof(Message.ReceiptHandle)], CancellationToken.None);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning($"HttpStatusCode: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }

        public async Task ChangeMessageVisibilityAsync(QueuedTaskDTO task, int visibilityTimeout, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug($"ChangeMessageVisibilityAsync : {task.TaskId} {visibilityTimeout}");

            var request = new ChangeMessageVisibilityRequest
            {
                QueueUrl = this[AwsConstants.QueueName],
                ReceiptHandle = task.Metadata[nameof(Message.ReceiptHandle)],
                VisibilityTimeout = visibilityTimeout
            };

            try
            {
                var response = await _sqsClient.ChangeMessageVisibilityAsync(request, token);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning($"HttpStatusCode: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }
    }
}
