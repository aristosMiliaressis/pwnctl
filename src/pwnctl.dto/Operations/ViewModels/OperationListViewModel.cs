namespace pwnctl.dto.Operations.ViewModels;

using pwnctl.app.Operations.Entities;

public sealed class OperationListViewModel
{
    public List<Operation> Operations { get; init; }

    public OperationListViewModel(List<Operation> operations)
    {
        Operations = operations;
    }
}