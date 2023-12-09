namespace pwnctl.app.Operations.Interfaces;

using pwnctl.app.Operations.Entities;

public interface OperationStateSubscriptionService
{
    Task Subscribe(Operation op);
    Task Unsubscribe(Operation op);
}

