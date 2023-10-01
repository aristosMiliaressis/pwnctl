namespace pwnctl.app.Queueing.Interfaces;

public interface TaskQueueService
{
    /// <summary>
    /// pushes a task to the pending queue.
    /// </summary>
    /// <param name="task"></param>
    Task EnqueueAsync<TMessage>(TMessage msg, CancellationToken token = default) where TMessage : QueueMessage;
    Task<TMessage?> ReceiveAsync<TMessage>(CancellationToken token = default) where TMessage : QueueMessage;

    Task DequeueAsync(QueueMessage msg);
    Task ChangeMessageVisibilityAsync(QueueMessage msg, int visibilityTimeout, CancellationToken token = default);

    Task Purge<TMessage>(CancellationToken token = default);
}

