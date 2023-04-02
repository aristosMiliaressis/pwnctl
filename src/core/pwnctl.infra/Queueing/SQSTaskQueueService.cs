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
        private string this[string messageType]
        {
            get
            {
                if (_queueUrls == null)
                    _queueUrls = new();

                var queueName = messageType == nameof(PendingTaskDTO)
                            ? PwnInfraContext.Config.TaskQueue.Name
                            : PwnInfraContext.Config.OutputQueue.Name;

                if (!_queueUrls.TryGetValue(queueName, out string queueUrl))
                {
                    var queueUrlResponse = _sqsClient.GetQueueUrlAsync(PwnInfraContext.Config.TaskQueue.Name).Result;
                    queueUrl = queueUrlResponse.QueueUrl;
                    _queueUrls[messageType] = queueUrl;
                }

                return queueUrl;
            }
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task<bool> EnqueueAsync(QueueMessage message, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug($"Enqueue: {message.TaskId}");

            var request = new SendMessageRequest
            {
                MessageGroupId = message.TaskId.ToString(),
                QueueUrl = this[message.GetType().Name],
                MessageBody = PwnInfraContext.Serializer.Serialize(message)
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

        public async Task<TMessage> ReceiveAsync<TMessage>(CancellationToken token = default) where TMessage : QueueMessage
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = this[typeof(TMessage).Name],
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
                    var task = PwnInfraContext.Serializer.Deserialize<TMessage>(msg.Body);
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
                return default(TMessage);
            }
        }

        public async Task DequeueAsync(QueueMessage message)
        {
            PwnInfraContext.Logger.Debug($"Dequeueing : {message.TaskId}");

            try
            {
                var response = await _sqsClient.DeleteMessageAsync(message.GetType().Name, message.Metadata[nameof(Message.ReceiptHandle)], CancellationToken.None);
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

        public async Task ChangeMessageVisibilityAsync(QueueMessage message, int visibilityTimeout, CancellationToken token = default)
        {
            PwnInfraContext.Logger.Debug($"ChangeMessageVisibilityAsync : {message.TaskId} {visibilityTimeout}");

            var request = new ChangeMessageVisibilityRequest
            {
                QueueUrl = this[message.GetType().Name],
                ReceiptHandle = message.Metadata[nameof(Message.ReceiptHandle)],
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
