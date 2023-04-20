namespace pwnctl.dto.Operations.Models;

using pwnctl.app.Operations.Entities;

public sealed class OperationListViewModel
{
    public List<Operation> Operations { get; init; }

    public OperationListViewModel(List<Operation> operations)
    {
        Operations = operations;
    }
}