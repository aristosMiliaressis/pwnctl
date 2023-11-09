using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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
        private readonly AmazonSQSClient _sqsClient  = new();
        private Dictionary<string,string> _queueUrls = new();
        private string this[string messageType]
        {
            get
            {
                if (!_queueUrls.ContainsKey(messageType))
                {
                    var queueName = messageType == nameof(PendingTaskDTO)
                                ? PwnInfraContext.Config.TaskQueue.Name
                                : PwnInfraContext.Config.OutputQueue.Name;

                    _queueUrls[messageType] = _sqsClient.GetQueueUrlAsync(queueName).Result.QueueUrl;
                }

                return _queueUrls[messageType];
            }
        }

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync<TMessage>(TMessage message)
            where TMessage : QueueMessage
        {
            PwnInfraContext.Logger.Debug("Enqueue["+typeof(TMessage).Name +"]: "+message.Metadata["MessageGroupId"]);

            try
            {
                var request = new SendMessageRequest
                {
                    MessageGroupId = message.Metadata["MessageGroupId"],
                    QueueUrl = this[typeof(TMessage).Name],
                    MessageBody = PwnInfraContext.Serializer.Serialize(message)
                };

                var response = await _sqsClient.SendMessageAsync(request);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning("HttpStatusCode: "+response.HttpStatusCode);
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }

        public async Task<TMessage> ReceiveAsync<TMessage>(CancellationToken token = default)
            where TMessage : QueueMessage
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = this[typeof(TMessage).Name],
                    MaxNumberOfMessages = 1
                };

                var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, token);
                if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(messageResponse));
                    PwnInfraContext.Logger.Warning("HttpStatusCode: "+messageResponse.HttpStatusCode);
                }

                return messageResponse.Messages.Select(msg =>
                {
                    var task = PwnInfraContext.Serializer.Deserialize<TMessage>(msg.Body);
                    PwnInfraContext.Logger.Debug("Received[" + typeof(TMessage).Name + "] : " + task.TaskId + ", MessageId: " + msg.MessageId);

                    task.Metadata = new Dictionary<string, string>
                    {
                        { nameof(msg.MessageId), msg.MessageId },
                        { nameof(msg.ReceiptHandle), msg.ReceiptHandle }
                    };

                    return task;
                }).FirstOrDefault();
            }
            catch (OperationCanceledException)
            {
                return default(TMessage);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return default(TMessage);
            }
        }

        public async Task DequeueAsync(QueueMessage message)
        {
            PwnInfraContext.Logger.Debug("Dequeue[" + message.GetType().Name + "]: " + message.TaskId);

            try
            {
                var response = await _sqsClient.DeleteMessageAsync(this[message.GetType().Name], message.Metadata[nameof(Message.ReceiptHandle)]);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning("HttpStatusCode: "+response.HttpStatusCode);
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }

        public async Task ChangeMessageVisibilityAsync(QueueMessage message, int visibilityTimeout)
        {
            PwnInfraContext.Logger.Debug("ChangeMessageVisibilityAsync[" + message.GetType().Name + "]: " + message.TaskId +" "+visibilityTimeout);

            try
            {
                var request = new ChangeMessageVisibilityRequest
                {
                    QueueUrl = this[message.GetType().Name],
                    ReceiptHandle = message.Metadata[nameof(Message.ReceiptHandle)],
                    VisibilityTimeout = visibilityTimeout
                };

                var response = await _sqsClient.ChangeMessageVisibilityAsync(request);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                    PwnInfraContext.Logger.Warning("HttpStatusCode: "+response.HttpStatusCode);
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
            }
        }

        public async Task Purge<TMessage>()
        {
            var queueUrl = this[typeof(TMessage).Name];

            var response = await _sqsClient.PurgeQueueAsync(queueUrl);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                PwnInfraContext.Logger.Warning(PwnInfraContext.Serializer.Serialize(response));
                PwnInfraContext.Logger.Warning("HttpStatusCode: "+response.HttpStatusCode);
            }
        }
    }
}
